   M S B A M L   `   `   `  ����  
Project F8,    &Project_F8.SupportWindows.DialogWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  dialogWindow��5   	    � Title$ Dialogs��5   	   $	��316��6   $	��589��6&    � 
ResizeMode$ NoResize=�6Q   .��5      � .+�5   
   �� -   5         ��Name$ 
messageBox��5      $
��Left=�6'   $	��214��6B   $��	10,10,0,0q�6O   $	��Top=�6b   $	��386��6z   $��Disabled=�6�    �� View 5      �� 5       �� AllowsColumnReorder .  .P�5	      �� .��5
      �� -   5         ��Name$ messageHeader��5
   /   $	��378��6F   $��Messages��6R   {�.  $��0q�6w   5      5      5      �� 5   
   -   5        $ 	dialogBox��5      $
��Left=�6&   $	��214��6A   $��
411,10,0,0q�6N   $	��Top=�6b   $	��160��6z   $��Disabled=�6�    � ContextMenu 5      �� 5      .@�5      m� -   5         m�Name$ addItem��5      $	��Add��60   {�. 6W   m� 5      -   5        $ makeFriendItem��5      $��Make friend��67   {�. 6l   m� 5      -   5        $ 	closeItem��5      $��Close��62   {�. 6]   �� 5      6"   5       5      �� 5       .  .P�5      �� .��5      �� -   5        $ dialogHeader��5   /   $	��153��6E   $��Dialogs��6Q   {�.  $��0q�6u   5      5      5      �� 5    
   -	   5        	 ��Name$	 textBox��5       $
��Left=�6#   $��36��6>   $��
10,229,0,0q�6J   
 �� TextWrapping$

 Wrap=�6^   $	��Top=�6r   $	��386��6�   6�   �� 5!   
   -
   5         ��Name$ 
sendButton��5!      $
��Send��6%   $
��Left=�64   $��36��6O   $��411,229,0,0q�6[   $	��Top=�6p   $	��160��6�    �� 	IsDefault . 6�   �� 5"   
   -   5         ��Name$ template��5"      $��Label��6"   $
��Left=�62   $	��Top=�6M    J� Opacity$ 0Q�6e   $��15,0,0,0q�6q   5$      5%      