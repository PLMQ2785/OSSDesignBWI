   M S B A M L   `   `   `  ���� J  FopenMediaPlayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:openMediaPlayeropenMediaPlayer  J FLibVLCSharp.WPF, Version=3.9.3.0, Culture=neutral, PublicKeyToken=nullJ6clr-namespace:LibVLCSharp.WPF;assembly=LibVLCSharp.WPFLibVLCSharp.WPF      openMediaPlayer.MainWindow   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     /localclr-namespace:openMediaPlayer     Fvlc6clr-namespace:LibVLCSharp.WPF;assembly=LibVLCSharp.WPF     -     � Title$  Open Media Player��$	��720��$
��1280�� � Icon$ /OMP.ico�� � Topmost �� *Settings.Playback.AlwaysOnTop+ �� Mode$ OneWay=�.���  � ColumnDefinitions �� $	��250 �$	��150��$	��400���� $��5 ��� $��* �.+�� $��0�� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto ��� $��* �.+��� $��0��$��5q�.+��� $��재생목록��$
��Bold"�$��Center=���  �� Orientation$ 
Horizontal=�$��Center=�$��0,5,0,0q�.+��� $��파일추가��$��2q����� *AddFilesToPlaylistCommand+�� $��제거��$��2q����� *RemoveSelectedCommand+�� $��	초기화��$��2q����� *ClearPlaylistCommand+�� $��1�� �� SelectionMode$ Single=�$��5,0,5,5q����� *PlaylistItems+ �� SelectedItem �� *SelectedPlaylistItem+	 J� InputBindings	 d� 
 d� MouseAction$
 LeftDoubleClicke� �� Command �� *PlaySelectedCommand+����  �� 
TargetType" ��.���  �� Property � �� �� Value$ Stretch=� �� Triggers ��  �� Binding �� *	IsPlaying+ �� Value True.a���  � ��$
 Bold"���  � ��
 ��ح� �� View �� .P��� $��Track�� �� Width$	 220�� �� DisplayMemberBinding �� *DisplayName+�� $��2��$ 
Horizontal=�$��Center=�$��5q�.+��� $��이전��$��5q����� *PreviousTrackCommand+�� $��다음��$��5q����� *NextTrackCommand+�� $��3��$��실시간 지원��$��5q�.���  �� $��* ��� $
��Auto �.+��� $��0��$
��Auto=�.���� $
��Wrap=�$��5q����� *
LlmHistory+� $��1�� �� $��* ��� $
��Auto �.+��� $��0��$��Center=����� *LlmInputText+ �� UpdateSourceTrigger$ PropertyChanged=�{��� *IsLlmInputEnabled+�� $��1��$��	보내기��$��5,0,0,0q����� *SubmitLlmCommand+{��� *IsLlmInputEnabled+�� $��1��$��5��$��Center=�$��Stretch=�
������� �� ShowsPreview$
 True��� $��2�� �� $��* ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+�  LibVLCSharp.WPF.VideoView  -     Name$ 	videoView��$��0��
���   �� $��1��$��5,0,5,5q� �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+��� -    ��Name$ CurrentTimeLabel��$��0��$��Center=�$��0,0,5,0q����� *CurrentTimeText+�� -    ��Name$ PositionSlider��$��1�� � Minimum$ 0Q� � SmallChange$
 0.01Q� � LargeChange$	 0.1Q�$��0,0,0,5q� � Value �� *SliderPosition+$ TwoWay=� � Maximum �� *SliderMaxValue+{��� *IsMediaLoaded+�� -   $ TotalDurationLabel��$��2��$��Center=�$��0,0,5,0q����� *TotalDurationText+� $��2��$��5q� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��파일 열기��$
��10,5q�$��Center=����� *OpenCommand+�� $��1��$ 
Horizontal=�$
��10,0q�$��Center=�.+��� $��재생��$
��10,5q�$��5,0,5,0q����� *PlayCommand+�� $��일시정지��$
��10,5q�$��0,0,5,0q����� *PauseCommand+�� $��정지��$
��10,5q����� *StopCommand+�� $��2��$ 
Horizontal=�$��Right=�.+��� -    ��Name$ 
MuteButton��$��30��$��30��$��5q�$��Center=�$��0,0,5,0q����� *MuteCommand+.���� ���� " ��.���  � ��$
 🔊�� ��  �� *	IsMuted+ True.a���  � ��$
 🔇���� -   $ VolumeSlider��$	��100��$��Center=�$ 0Q�$	 100Q�$��0,0,10,0q� �� *Volume+$ TwoWay=�{��� *IsMediaLoaded+�� -   $ GenerateSubtitlesButton��$��자막 생성��$
��10,5q�$��Center=�{��� *CanGenerateSubtitles+���� *GenerateSubtitlesCommand+�� -	   $ SettingsButton��$��설정��$
��10,5q�$��Center=�$��5,0,0,0q����� *OpenSettingsWindowCommand+�� $��3��.@��� .���� ���� *StatusMessage+