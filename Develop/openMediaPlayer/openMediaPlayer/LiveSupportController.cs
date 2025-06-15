using LLama;
using LLama.Common;
using LLama.Sampling;
using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace openMediaPlayer.Services
{
    //LLM JSON 응답 파싱 클래스
    public record LlmActionResponse(string action, Dictionary<string, object> parameters);

    public class LiveSupportController : ILiveSupportController
    {
        private readonly IPreferencesController _preferencesController;
        private readonly IPlayerActionRegistry _actionRegistry;
        private readonly IDispatcherController _dispatcherController;

        // LlamaSharp 객체들
        private LLamaWeights? _modelWeights;
        private LLamaContext? _context;
        private ChatSession? _session;

        public bool IsInitialized { get; private set; } = false;

        public LiveSupportController(
            IPreferencesController preferencesController,
            IPlayerActionRegistry actionRegistry,
            IDispatcherController dispatcherController)
        {
            _preferencesController = preferencesController;
            _actionRegistry = actionRegistry;
            _dispatcherController = dispatcherController;
        }

        public async Task InitializeAsync()
        {
            if (IsInitialized) return;

            await Task.Run(() =>
            {
                try
                {
                    var parameters = new ModelParams(_preferencesController.llmModelPath)
                    {
                        ContextSize = 1024
                    };
                    _modelWeights = LLamaWeights.LoadFromFile(parameters);
                    _context = _modelWeights.CreateContext(parameters);

                    var executor = new InteractiveExecutor(_context);
                    _session = new ChatSession(executor);

                    // AuthorRole은 LLama.Common 네임스페이스에 있음
                    _session.History.AddMessage(AuthorRole.System, BuildSystemPrompt());

                    IsInitialized = true;
                    System.Diagnostics.Debug.WriteLine("LLM Model Initialized Successfully.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"LLM Initialization Failed: {ex.Message}");
                    IsInitialized = false;
                }
            });
        }

        public async Task<string> ProcessUserInputAsync(string userInput)
        {
            if (!IsInitialized || _session == null)
            {
                return "LLM is not initialized.";
            }

            try
            {
                var userMessage = new ChatHistory.Message(AuthorRole.User, userInput);

                var fullResponse = new StringBuilder();

                var inferenceParams = new InferenceParams()
                {
                    MaxTokens = 128,
                    AntiPrompts = new List<string> { "User:" },
                    SamplingPipeline = new DefaultSamplingPipeline()
                    {
                        Temperature = 0.1f
                    }
                };

                await foreach (var text in _session.ChatAsync(userMessage, inferenceParams))
                {
                    fullResponse.Append(text);
                }

                var rawResponse = fullResponse.ToString();
                System.Diagnostics.Debug.WriteLine($"LLM Raw Response: [{rawResponse}]");

                int firstBrace = rawResponse.IndexOf('{');
                if (firstBrace == -1)
                {
                    return "LLM did not return a valid JSON object (no starting brace).";
                }

                int braceCount = 0;
                int lastBrace = -1;
                for (int i = firstBrace; i < rawResponse.Length; i++)
                {
                    if (rawResponse[i] == '{')
                    {
                        braceCount++;
                    }
                    else if (rawResponse[i] == '}')
                    {
                        braceCount--;
                    }

                    if (braceCount == 0)
                    {
                        lastBrace = i;
                        break; // 첫 번째로 완성된 JSON 객체의 끝을 찾음
                    }
                }

                if (lastBrace == -1)
                {
                    return "LLM did not return a valid JSON object (unbalanced braces).";
                }

                string responseJson = rawResponse.Substring(firstBrace, lastBrace - firstBrace + 1);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var llmAction = JsonSerializer.Deserialize<LlmActionResponse>(responseJson, options);

                if (llmAction != null && !string.IsNullOrEmpty(llmAction.action) && llmAction.action != "unknown")
                {
                    //string result = "Action could not be executed.";
                    //await _dispatcherController.InvokeAsync(async () =>
                    //{
                    //    result = await _actionRegistry.ExecuteActionAsync(llmAction.action, llmAction.parameters ?? new());
                    //});
                    //return result;

                    string result = await _dispatcherController.InvokeAsync(async () =>
                    {
                        return await _actionRegistry.ExecuteActionAsync(llmAction.action, llmAction.parameters ?? new());
                    });
                    return result;

                }
                return "Could not understand the command.";
            }
            catch (Exception ex)
            {
                return $"An error occurred during LLM processing: {ex.Message}";
            }
        }

        private string BuildSystemPrompt()
        {
            //var availableActions = new[] { "play", "pause", "stop", "next_track", "previous_track", "generate_subtitles" };
            //var actionList = string.Join(", ", availableActions.Select(a => $"'{a}'"));

            return $$$"""
                    You are an expert media player assistant. 
                    Your task is analyze the user's request and respond with a single JSON object.

                    Your response MUST be only the JSON object and nothing else.
                    The JSON object must have an "action" field and a "parameters" field.
                    You MUST choose a value for the "command_name" field from this exact list: "play", "pause", "stop", "next_track", "previous_track", "generate_subtitles", "preferences".
                    You MUST respond like this format -> {"action": "command_name", "parameters": {"key":"value"}}
                    If the request is unclear or cannot be mapped to a command from the list, use the "unknown" action.

                    Do not add any conversational text, explanations, or any characters outside of the single JSON object.
                    """;
        }
    }
}