   M S B A M L   `   `   `  ���� O  KUnnamedSstrategyGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:UnnamedStrategyGame.UIUnnamedStrategyGame.UI  &     UnnamedStrategyGame.UI.MapEditor   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6local$clr-namespace:UnnamedStrategyGame.UI     -     N� Title$  	MapEditor��.��� .+��� -    ��Name$ mapCanvasContainer��.+��� -   $ 	mapCanvas��� 
�������$
��Left=�$	��Top=� � ColumnDefinitions �� $��2* ���  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��* �.+���  �� Orientation$ 
Horizontal=�$��0��$��2��.+��� -   .��New�� -   .��Load�� -   .��Save�� $��3��.��Author�� -    ��Name$ 	optAuthor��$��1��$��3���� $��5��.��
Commanders]� $��5��$��1��.+��� -   $��Right=�$��5,0,5,0q�.��-�� -	   $��Right=�$��5,0,5,0q�.��+�� -
    ��Name$ lblCommanders��.��0�� $��10��$��2��.��Placement Mode�� -   $ lblPlacementMode��$��11���� -   $��1��$��11��.��Change�� -    ��Name$ optMirrorHorizontally��$��2��$��12��.��Mirror Horizontally�� -   $ optMirrorVertically��$��2��$��13��.��Mirror Vertically�� -   $ optMirrorDiagonally��$��2��$��14��.��Mirror Diagonally�� -   $ optMirrorCommanders��$��2��$��15��.��Mirror Commanders�� -   $��2��$��20��.��Back�� -    ��Name$ placementModeSelectBorder��
�������$y�	Collapsed=�._�9   3UnnamedStrategyGame.UI.MapEditorPlacementModeEditor  -   	  Name$	 placementModeSelect��9   3UnnamedStrategyGame.UI.MapEditorPlacementModeEditor
   ModeSelected$&
  placementModeSelect_ModeSelected��   Cancel$  placementModeSelect_Cancel���� -   $ mapSizeEditorBorder��
�������$y�	Collapsed=�._�*   $UnnamedStrategyGame.UI.MapSizeEditor  -     Name$ mapSizeEditor��*   $UnnamedStrategyGame.UI.MapSizeEditor   Saved$ mapSizeEditor_Saved��   Closed$ mapSizeEditor_Closed��