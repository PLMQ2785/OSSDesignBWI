   M S B A M L   `   `   `  ����J  FopenMediaPlayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:openMediaPlayeropenMediaPlayer  5   	   J FLibVLCSharp.WPF, Version=3.9.3.0, Culture=neutral, PublicKeyToken=nullJ6clr-namespace:LibVLCSharp.WPF;assembly=LibVLCSharp.WPFLibVLCSharp.WPF 5   	        openMediaPlayer.MainWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:openMediaPlayer     5   	   Fvlc6clr-namespace:LibVLCSharp.WPF;assembly=LibVLCSharp.WPF     5   	   -   5          � Title$  Open Media Player��5	   	   $	��720��6#   $
��1280��60    � Icon$ /OMP.ico��5   	    � Topmost 5   	   �� *Settings.Playback.AlwaysOnTop+ �� Mode$ OneWay=�.��5      �  � ColumnDefinitions 5   
   �� 5      $	��250 �6   $	��150��6+   $	��400��6:   �� 5      $��5 �6   �� 5      $��* �6   .+�5   
   � $��0��6    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    �� 5      $
��Auto �6    �� 5      $��* �6    .+�5      �� $��0��6   $��5q�6&   .+�5      �� $��Playlist��6   $
��Bold"�6+   $��Center=�6=   �� 5       �� Orientation$ 
Horizontal=�6   $��Center=�66   $��0,5,0,0q�6S   .+�5       �� $��	Add Files��6   $��2q�5!      ��5    1   �� *AddFilesToPlaylistCommand+5!      �� 5"      $��Remove��6   $��2q�6X   ��6.   �� *RemoveSelectedCommand+6X   �� 5#      $��Clear��6   $��2q�6V   ��6-   �� *ClearPlaylistCommand+6V   5$      5%      �� 5'      $��1��6    �� SelectionMode$ Single=�5*      $��5,0,5,5q�5+      ��5(      �� *PlaylistItems+ �� SelectedItem 5)      �� *SelectedPlaylistItem+	 J� InputBindings	 5-      d� 5.      
 d� MouseAction$
 LeftDoubleClicke�6#    �� Command 5/      �� *PlaySelectedCommand+��52      �� 53       �� 
TargetType" ��6   .�54      ��  �� Property � �� �� Value$ Stretch=�6G    �� Triggers 55      �� 56       �� Binding 6*   �� *	IsPlaying+ �� Value True6H   .a�57   "   ��  � ��$
 Bold"�6?   �� 58   "    � ��
 ��ح�6?   59      5;       �� View 5>      �� 5?      .P�5@      �� $��Track��6)    �� Width$	 220��5A       �� DisplayMemberBinding 5@   8   �� *DisplayName+5A      5B      5D      �� 5F      $��2��6   $ 
Horizontal=�6&   $��Center=�6?   $��5q�5G      .+�5H      �� $
��Prev��6   $��5q�6Q   ��6(   �� *PreviousTrackCommand+6Q   �� 5I      $
��Next��6   $��5q�6M   ��6(   �� *NextTrackCommand+6M   5J      �� 5L      $��3��6   $��Live Support��6$   $��5q�6:   .��5M      �  5N      �� 5O      $��* �6(   �� 5P      $
��Auto �6(   .+�5R      �� $��0��6#   $
��Auto=�60   .��5S      �� $
��Wrap=�6@   $��5q�6T   ��6$   �� *
LlmHistory+6T   5T      � 5U      $��1��6    5V      �� 5W      $��* �6/   �� 5X      $
��Auto �6/   .+�5Z      �� $��0��6"   $��Center=�6u   ��62   �� *LlmInputText+ �� UpdateSourceTrigger$ PropertyChanged=�{�6�   �� *IsLlmInputEnabled+�� 5[      $��1��6!   $
��Send��61   $��5,0,0,0q�6e   ��6@   �� *SubmitLlmCommand+{�6v   �� *IsLlmInputEnabled+5\      5]      5^      5_      �� 5f   
   $��1��6   $��5��5g      $��Center=�5h      $��Stretch=�5i      
������� �� ShowsPreview$
 True��5k      � 5m   
   $��2��6    5o      �� 5p      $��* �6    �� 5q      $
��Auto �6    �� 5r      $
��Auto �6    �� 5s      $
��Auto �6    .+�5v        LibVLCSharp.WPF.VideoView  -   5          Name$ 	videoView��5v      $��0��6/   
���   �6<   � 5y      $��1��6   $��5,0,5,5q�6     5{      �� 5|      $
��Auto �6'   �� 5}      $��* �6'   �� 5~      $
��Auto �6'   �� 5      $
��Auto �6'   .+�5�      �� -   5         ��Name$ CurrentTimeLabel��5�      $��0��66   $��Center=�6g   $��0,0,5,0q�6�   ��6F   �� *CurrentTimeText+6�   �� 5�      -   5         ��Name$ PositionSlider��5�      $��1��5�       � Minimum$ 0Q�5�       � SmallChange$
 0.01Q�5�       � LargeChange$	 0.1Q�5�      $��0,0,0,5q�5�       � Value 5�      �� *SliderPosition+$ TwoWay=� � Maximum 5�      �� *SliderMaxValue+{�5�      �� *IsMediaLoaded+�� 5�      -   5        $ TotalDurationLabel��5�      $��2��68   $��Center=�6k   $��0,0,5,0q�6�   ��6H   �� *TotalDurationText+6�   5�      � 5�      $��2��6   $��5q�6     5�      �� 5�      $
��Auto �6'   �� 5�      $��* �6'   �� 5�      $
��Auto �6'   .+�5�      �� $��0��6   $��파일 열기��6)   $
��10,5q�69   $��Center=�6H   ��6c   �� *OpenCommand+�� 5�      $��1��6   $ 
Horizontal=�6-   $
��10,0q�6F   $��Center=�6T   .+�5�      �� $��재생��6   $
��10,5q�6*   $��5,0,5,0q�69   ��6J   �� *PlayCommand+�� 5�      $��일시정지��6   $
��10,5q�6,   $��0,0,5,0q�6;   ��6L   �� *PauseCommand+�� 5�      $��정지��6   $
��10,5q�6*   ��69   �� *StopCommand+5�      �� 5�      $��2��6   $ 
Horizontal=�6-   $��Right=�6F   .+�5�      �� -   5         ��Name$ 
MuteButton��5�      $��30��6Q   $��30��6\   $��5q�6h   $��Center=�6t   $��0,0,5,0q�6�   ��61   �� *MuteCommand+.��5�      �� ��5�      �� 5�   "   " ��6(   .�5�   &   ��  � ��$
 🔊��6=    5�   &   �� 5�   *    66   �� *	IsMuted+ True6R   .a�5�   .   ��  � ��$
 🔇��6E   5�   +   5�   #   5�      5�      �� 5�      -   5        $ VolumeSlider��5�      $	��100��63   $��Center=�6?   $ 0Q�5�      $	 100Q�6)   $��0,0,10,0q�5�   A    5�      �� *Volume+$ TwoWay=�{�5�      �� *IsMediaLoaded+6A   �� 5�      -   5        $ GenerateSubtitlesButton��5�      $��자막 생성��6>   $
��10,5q�6N   $��Center=�6]   {�6x   �� *CanGenerateSubtitles+��6�   �� *GenerateSubtitlesCommand+�� 5�      -	   5        $ SettingsButton��5�      $��설정��5�      $
��10,5q�5�      $��Center=�6(   $��5,0,0,0q�5�      ��5�      �� *OpenSettingsWindowCommand+5�      5�      �� 5�      $��3��6   .@�5�      �� .��5�      �� ��6    �� *StatusMessage+5�      5�      5�      5�      5�      