   M S B A M L   `   `   `  ����J  FopenMediaPlayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:openMediaPlayeropenMediaPlayer  5   	   J FLibVLCSharp.WPF, Version=3.9.3.0, Culture=neutral, PublicKeyToken=nullJ6clr-namespace:LibVLCSharp.WPF;assembly=LibVLCSharp.WPFLibVLCSharp.WPF 5   	        openMediaPlayer.MainWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:openMediaPlayer     5   	   Fvlc6clr-namespace:LibVLCSharp.WPF;assembly=LibVLCSharp.WPF     5   	     � Title$  Open Media Player��5	   	   $	��720��6#   $
��1280��60   .��5
      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
     LibVLCSharp.WPF.VideoView  -   5          Name$ 	videoView��5      $��0��6+   �� 5   
   $��1��6    �� Orientation$ Vertical=�6"   $��10q�69   .+�5      � $��0,0,0,5q�6    � ColumnDefinitions 5      �� 5      $
��Auto �6'   �� 5      $��* �6'   �� 5      $
��Auto �6'   .+�5      �� -   5         ��Name$ CurrentTimeLabel��5      $��0��66   $��Center=�6g   $��0,0,5,0q�6�   ��6F   �� *CurrentTimeText+6�   �� 5      -   5         ��Name$ PositionSlider��5      $��1��5       � Minimum$ 0Q�5!       � SmallChange$
 0.01Q�5#      	 � LargeChange$		 0.1Q�5$      $��0,0,0,5q�5%      
 � Value
 5       �� *PositionSlider+ �� Mode$ TwoWay=� � Maximum 5"      �� *SliderMaxValue+{�5(      �� *IsMediaLoaded+�� 5)      -   5        $ TotalDurationLabel��5)      $��2��68   $��Center=�6k   $��0,0,5,0q�6�   ��6H   �� *TotalDurationText+6�   5,      � 5.       50      �� 51      $
��Auto �6'   �� 52      $��* �6'   �� 53      $
��Auto �6'   .+�56      �� $��0��6   $��파일 열기��6)   $
��10,5q�69   $��Center=�6H   ��6c   �� *OpenCommand+�� 58      $��1��6   $ 
Horizontal=�6-   $��10q�6F   $��Center=�6R   .+�5;      �� $��재생��6   $
��10,5q�6*   $��5,0,5,0q�69   ��6J   �� *PlayCommand+�� 5<      $��일시정지��6   $
��10,5q�6,   $��0,0,5,0q�6;   ��6L   �� *PauseCommand+�� 5=      $��정지��6   $
��10,5q�6*   ��69   �� *StopCommand+5>      �� 5@      -   5         ��Name$ GenerateSubtitlesButton��5@   )   $��2��6   $��자막 생성��6J   $
��10,5q�6Z   $��Center=�6i   {�6�   �� *CanGenerateSubtitles+��6�   �� *GenerateSubtitlesCommand+5B      5E      5F      5H      