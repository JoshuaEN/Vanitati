   M S B A M L   `   `   `  ���� O  KUnnamedSstrategyGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:UnnamedStrategyGame.UIUnnamedStrategyGame.UI  )    #UnnamedStrategyGame.UI.BattleViewV2   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6local$clr-namespace:UnnamedStrategyGame.UI     -     N� Title$  BattleViewV2��.��� -    �Name$ rootContainer��
�������.+��� -    ��Name$ ContainerCanvas��.+��� -   $ BattleCanvas�� �� EdgeMode$ Unspecified=� � UseLayoutRounding .  �� BitmapScalingMode$ Linear=��� -   $ AllyOverlayCanvas��$ Unspecified=� .  J� IsHitTestVisible .   J� 	CacheMode &   System.Windows.Media.BitmapCache  �� -   $ EnemyOverlayCanvas��$ Unspecified=� .  .     �� -   $ TerrainOverlayCanvas��$ Unspecified=� .  .     �� -   $ NeutralOverlayCanvas��$ Unspecified=� .  .     �� -	   $' !SelectionUnavailableOverlayCanvas��$ Unspecified=� .  .     �� -
   $ UnitExpendedOverlayCanvas��$ Unspecified=� .  .     �� ���	#FF858585$��0,2,2,0q�$
��Left=�$��Bottom=�
�������._�� -   $% selectedItemActionViewContainer���� ���	#FF858585$��0,0,2,2q�$
��Left=�$	��Top=�
�������._�� -   $) #currentCommanderActionViewContainer���� ���	#FF858585$��2,2,0,0q�$��Right=�$��Bottom=�
������� �� Padding$ 0,0,0,5q�._��� .+��� ���	#FF858585$��0,0,0,1q�$ 0,0,0,2q�._��� 	 �� TextAlignment$	 Center=�Hovered Unit%   UnnamedStrategyGame.UI.UnitView  -   
  Name$
 unitHoveredStatePropView�� .  
��������� ���	#FF858585$��0,0,0,1q�$ 0,0,0,10q�._��� $	 Center=�Hovered Terrain(   "UnnamedStrategyGame.UI.TerrainView  -     Name$! terrainHoveredStatePropView�� .  
��������� ���	#FF858585$��2,0,0,2q�$��Right=�$	��Top=�
�������._���  �� Orientation$ 
Horizontal=�.+��� -   .��Menu+   %UnnamedStrategyGame.UI.CommandersView  -     Name$ commandersListing��� -   $ inputPromptGrid��$y�	Collapsed=����	#CC000000.+��� $��Center=�$��Center=�
�������.��(   "UnnamedStrategyGame.UI.InputPrompt  -     Name$ inputPrompt��(   "UnnamedStrategyGame.UI.InputPrompt   OptionSelected$  inputPrompt_OptionSelected���� -   $��Bottom=�$��Right=�.��Cancel� -   $" freeformInputPromptContainer��$y�	Collapsed=����	#CC000000.+�� -   $ freeformInputPromptGrid��
��������� -   $��Bottom=�$��Right=�.��Cancel�� -    ��Name$# userToCommanderAssignmentGrid��$y�	Collapsed=����	#7F000000._��� $��Center=�$��Center=�.+�6   0UnnamedStrategyGame.UI.UserToCommanderAssignment  -     Name$ userToCommanderAssignment��6   0UnnamedStrategyGame.UI.UserToCommanderAssignment   ReleaseCommand$. (userToCommanderAssignment_CommandTrigger��   TakeCommand$. (userToCommanderAssignment_CommandTrigger��$��Center=�$��Center=��� -   .��Close'	   !UnnamedStrategyGame.UI.BattleMenu	  -    	 Name$ 
battleMenu��'
   !UnnamedStrategyGame.UI.BattleMenu 
  SaveGame$ battleMenu_SaveGame�� 
  
ExitToMenu$ battleMenu_ExitToMenu�� 
  ExitToDesktop$ battleMenu_ExitToDesktop�� 
  ReturnToBattle$ battleMenu_ReturnToBattle�� 
  CommanderAssignment$$ battleMenu_CommanderAssignment��$y�	Collapsed=��� -   $ stateLoadingGrid��$y�	Collapsed=����	#7F000000._��� $��Center=�$��Center=�
�������.+��� -    ��Name$ stateLoadingStatusLabel��$��32(��� -   $% stateLoadingDetailedStatusLabel��$��20(��� -   $ disconnectedContainer��$y�	Collapsed=����	#7F000000._��� -    ��Name$ disconnectedPanel��$��Center=�$��Center=�
���ؿ��.+��� $��46(�.��Disconnected�� -    .��	Save Game�� -!   .��Exit to Main Menu�� -"   .��Exit to Desktop