   M S B A M L   `   `   `  ����  SocialRobot    SocialRobot.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Social Robot��5   	   $	��720��6   $
��1280��6+    � WindowStyle$
 None=�6R    � 
ResizeMode$ NoResize=�6e    � WindowStartupLocation$ CenterScreen=�5       � WindowState$ 	Maximized=�6*   .��5      � ��5   
   �� 5       �� ImageSource$ image/Background.png��6   .+�5
   
   �� -   5         ��Name$ Language_text��5
      $
��Left=�6+   $��	31,56,0,0q�6F   $
��Wrap=�6Y   $	��Top=�6m   $	��241��6�   $��65��6�   $��40(�6�   $��SketchFlow Print)�6�   �� 5   
   $
��Left=�6   $��
31,167,0,0q�6/   $
��Wrap=�6C   $��Switch Language��6W   $	��Top=�6n   $��20(�6�   $��Segoe Print)�6�   �� 5   
   -   5         ��Name$ SwitchLanguage��5      $�� ��6)   $
��Left=�64   $��
71,207,0,0q�6O   $	��Top=�6c   $��70��6{   $��70��6�   ��6�   Z� ��5      �� 5      $ image/Switch Language.png��6   5      �� 5   
   -   5        $ tb_result_text��5      $
��Left=�6,   $��
31,478,0,0q�6G   $
��Wrap=�6[   $	��Top=�6o   $	��337��6�   $	��167��6�   $��35(�6�   $��Kristen ITC)�6�    �� TextAlignment$ Center=�6�   � 5   
   -   5        	 �Name$	 Volume��5      $
��Left=�6&   $��77��6A   $��597,158,0,0q�6M   $	��Top=�6b   $��89��6z   
 J� RenderTransformOrigin$
 0.5,0.5&�6�    � Orientation$ Vertical=�6�   z�5      V� 5      .~�5      �� �� 5       �� AngleX$ -0.006Q�6$   �� 5       �� Angle$ 0.079Q�6&   T� 5      5      5      �� 5   
   $��507,115,503,338q�6   $��image/Microphone.png��6)   $
��Fill=�6G   $
 0.5,0.5&�6V   z�5      V� 5      .~�5      ��  �� ScaleX$ -1Q�6%   �� 5       �� 5!      T� 5"      5#      5%      �� 5&   
   -   5        $ UI��5&      $
��Left=�6    $��416,421,0,0q�6;   $
��Wrap=�6P   $	��Top=�6d   $	��224��6|   $	��457��6�   $��50(�6�   $��Segoe Print)�6�   $ Center=�6�   �� 5'   
   -   5        $	 Esc��5'      $�� ��6   $
��Left=�6)   $��1189,30,0,0q�6D   $	��Top=�6Y   $��60��6q   $��60��6|   $��20(�6�   ��6�   Z� ��5(      �� 5)      $ image/Power off.png��6   5+      �� 5,   
   -   5        $ Setting��5,      $��
��6"   $
��Left=�62   $��1110,25,0,0q�6M   $	��Top=�6b   $��65��6z   $��65��6�   ��6�   Z� ��5-      �� 5.      $ image/Setting.png��6   50      �� 51   
   -	   5         ��Name$ WeatherIcon��51      $
��Left=�52   
   $	��246��53   
   $��966,342,0,0q�54   
   $	��Top=�55   
   $	��270��56   
   &   System.Windows.Controls.Calendar  58   
   $
��Left=�6   $��1063,146,0,0q�6.   $	��Top=�6D   $	��176��6\   $	��186��6i   �� 59   
   -
   5        $ WeatherText��59      $
��Left=�6)   $��966,593,0,0q�6D   $
��Wrap=�6Y   $	��Top=�6m   $��68��6�   $	��270��6�   $��32(�6�   $��Segoe Print)�6�   $ Center=�6�   �� 5:   
   -   5        $�� ��5:      $
��Left=�6   $��60��67   $��1189,90,0,0q�6C   $	��Top=�6X   $��60��6p   ��5;      �� 5<      $ image/Health.png��6   5>      �� 5?   
   -   5        $�� ��5?      $
��Left=�6   $��60��67   $��1032,30,0,0q�6C   $	��Top=�6X   $��60��6p   $��22(�6�   ��5@      �� 5A      $ image/call.jpg��6   5C      �� 5D   
   -   5        $ Register��5D      $��R��6#   $
��Left=�6/   $��1032,90,0,0q�6J   $	��Top=�6_   $��60��6w   $��60��6�   $��36(�6�   6�   �� 5E   
   -   5        $
 Free��5E      $��F��6   $
��Left=�6+   $��
953,90,0,0q�6F   $	��Top=�6Z   $��62��6r   $��60��6}   $��41(�6�   6�   �� 5F   
   -   5        $ UserData��5F      $
��Left=�6&   $	��303��6A   $��1021,342,0,0q�6N   $
��Wrap=�6d   $	��Top=�6x   $	��228��6�   $��24(�6�   �� 5G   
   -   5        $ WakeUp��5G      $��Wake Up��6!   $
��Left=�63   $��60��6N   $��
869,90,0,0q�6Z   $	��Top=�6n   $��62��6�   6�   �� 5H   
   -   5        $ E��5H      $��E��6   $
��Left=�6(   $��58��6C   $��1110,92,0,0q�6O   $	��Top=�6d   $��62��6|   $��36(�6�   5J      5K      