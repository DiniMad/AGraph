   M S B A M L   `   `   `  ����A  =AGraph, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:AGraphAGraph  5   	       AGraph.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:AGraph     5   	   -   5          �Name$  WindowsMain��5   	    � Title$ A Graph��5   	   $	��500��6   $	��800��6&    � 
ResizeMode$ CanMinimize=�62    � WindowStyle$ 
ToolWindow=�6K    � WindowState$ 	Maximized=�6d    � WindowStartupLocation$ CenterScreen=�6|    � 	Resources 5	       �� 
TargetType   ButtonRadiusEffect �� Property	 �� Value
 �� 
TargetType �� CornerRadius ��Name 
 Vertex �� Triggers P� Property P� Value  FadeAnimation N� EnterActions �� TargetProperty W� From	 W� To g� Duration  ButtonCustomHoverBackground :� RoutedEvent	 �� To��%�  &	        &	     &	 �    &	 M    �� 5
   
   " ��6   .�5      ��  � ��	 5      �� 5      "
 ��6&   .R�5      �� $ 15��6!   #��z��63   ._�5      �� $ contentPresenter��6/   #��z��6I   #��z��6}   #��z��6�   #��z��6�   #��z��6  5      5      5      5      �� 5   
   " ��6   .�5      ��  � ��
	 �   �6+   �� 5       � ��	 5      �� 5      "
 ��6&   .R�5      �� $ 20��6!   #��z��63   #��z��6]   #��z��6�   ._�5      �� $��Center=�6/   $��Center=�6L   5      5      5       5       P� 5!       �@J�IsMouseOver . . �5"      ��  � ��
	 �����63   5#      5%      �� 5&   
   " �6    5'      P� 5(       � y�$ Visible=�60    5)      �� 5*      .��5+      �� .��5,   "   W� $ Opacity�62   $	 0.0X�5-   2   $	 1.0X�6=   $ 0:0:0.2D�6F   5.      5/      51      P� 52       � y�$ Hidden=�60    53      �� 54      .��55      �� .��56   "   W� $ Opacity�62   $	 1.0X�57   2   $	 0.0X�6=   $ 0:0:0.5D�6F   58      59      5;      5=      �� 5>   
   " ��6   .�5?      ��  � ��	 �	#FF4242426+   �� 5@       � ��	 5A      �� 5B      "
 ��6&   .R�5C      �� #��z��6!   ._�5D      �� $��Center=�6/   $��Center=�6L   5E      5F      5H       5I      :� 5J      $ 
MouseEnter��6   .[�5K      �� .��5L      �� .��5M      �� $ Background.Color�6-   $ 0:0:.5D�6Z   $ 	#FF838383X�5N   -   6>   5O      5P      5Q      :� 5R      $ 
MouseLeave��6   .[�5S      �� .��5T      �� .��5U      �� $ Background.Color�6-   $ 0:0:.5D�6Z   $ 	#FF424242X�5V   -   6>   5W      5X      5Y      5[      5\      .��5]      � -   5         �Name$ GridMain��5]       � ColumnDefinitions 5^   
   �� 5_      $	��135 �6   �� 5`      $
��642* �6   �� 5a      $	��135 �6   .+�5h   
   �� -   5         ��Name$# LblVerticesTextBoxPlaceHolder��5h      $��0��67   $��Number of Vertices��6G   $
��Left=�6d   $��25��6   $	��Top=�6�   $	��125��6�   $��5,10,5,0q�6�   $��Center=�6�   ���#A9A9A9$
��Bold"�6�   �� 5i   
   -   5         ��Name$ TxtVerticesNumber��5i      $��0��6-   $
��Left=�6=   $��25��6X    �� TextWrapping$
 Wrap=�6d   $�� ��6x   $	��Top=�6�   $	��125��6�   $��5,10,5,0q�6�   
������ $��Center=�6  $��Center=�6#  6G  �� 5j   
   -   5         ��Name$ BtnCreateGraph��5j      #��[ 6)   $��0��6^   
�������$��Create Graph��6�   $
��Left=�6�   $��5,40,5,0q�6�   $	��Top=�6�   $	��125��6�   $��24��6  � 5k   
   -   5        $ GridGraphOptions��5k      $��0��6   $y�Hidden=�69   #��[ 6M   .+�5l      �� -   5        $ BtnIsGraphConnected��5l      #��[ 62   $��0��6g   
�������$��Is Graph Connected��6�   $
��Left=�6�   $��5,70,5,0q�6�   $	��Top=�6�   $	��125��6�   $��24��6�   6  �� 5m      -   5        $ BtnIsGraphEulerian��5m      #��[ 61   $��0��6f   
�������$��Is Graph Eulerian��6�   $
��Left=�6�   $��	5,100,5,0q�6�   $	��Top=�6�   $	��125��6�   $��24��6�   6  �� 5n      -	   5        $ BtnStartGreedyColoring��5n      #��[ 65   $��0��6j   
�������$��Start Greedy Coloring��6�   $
��Left=�6�   $��	5,130,5,0q�6�   $	��Top=�6�   $	��125��6�   $��24��6�   6  �� 5o      -
   5        $ BtnStartOptimalColoring��5o      #��[ 66   $��0��6k   
�������$��Start Optimal Coloring��6�   $
��Left=�6�   $��	5,160,5,0q�6�   $	��Top=�6�   $	��125��6�   $��24��6  6  �� 5p      -   5        $ BtnIsGrapfBipartite��5p      #��[ 62   $��0��6g   
�������$��Is Grapf Bipartite��6�   $
��Left=�6�   $��	5,190,5,0q�6�   $	��Top=�6�   $	��125��6�   $��24��6�   6  �� 5q      -   5        $ BtnBuildAdjacencyMatrix��5q      #��[ 66   $��0��6k   
�������$��Build Adjacency Matrix��6�   $
��Left=�6�   $��	5,220,5,0q�6�   $	��Top=�6�   $	��125��6�   $��24��6  6  5r      � 5�   
   -   5        $ 	GridGraph��5�      $��3��6"   $��0��66   5�      �� 5�   
   $��0��6   $��Seyed Hossein Hosseini��6    $��Center=�6A   $	��135��6^   $��Bottom=�6j   $��5,0,5,40q�6�   $��Center=�6�   $��Center=�6�   �� 5�   
   $��0��6   $��
9450231110��6    $��Center=�65   $	��125��6R   $��Bottom=�6^   $��5,0,5,10q�6y   $��Center=�6�   $��Center=�6�   �� 5�   
   $��2��6   $��Ahmad AbrahimZade��6    $��Center=�6<   $	��135��6Y   $��Bottom=�6e   $��5,0,5,40q�6�   $��Center=�6�   $��Center=�6�   �� 5�   
   $��2��6   $��
9650231101��6    $��Center=�65   $	��125��6R   $��Bottom=�6^   $��5,0,5,10q�6y   $��Center=�6�   $��Center=�6�   � 5�   
   -   5        $ GridGreedyColoringInfo��5�      $��2��6/   $y�Hidden=�6?   #��[ 6S   $��	-10,0,0,0q�6z   .+�5�      �� $��Select a sequence��6   $��16(�60   $
��Bold"�6>   $��Center=�6P   $��Center=�6t   $	��Top=�6�   �� 5�      $��of (some) vertices��6   $��16(�61   $
��Bold"�6?   $��Center=�6Q   $��Center=�6u   $	��Top=�6�   $��0,30,0,0q�6�   �� 5�      $��and hit��6   $��16(�6&   $
��Bold"�64   $��Center=�6F   $��Center=�6j   $	��Top=�6�   $��0,60,0,0q�6�   �� 5�      -   5        $ BtnStartGreedyAlgorithm��5�      $��18(�66   $
��Bold"�6D   #��[  6V    � Cursor$
 Hand��6�   
���  ��
�������$��Start��6�   $��Center=�6�   $��Center=�6�   $	��Top=�6
  $	��100��6"  $��40��6.  $��0,95,0,0q�6:  6L  5�      5�      5�      