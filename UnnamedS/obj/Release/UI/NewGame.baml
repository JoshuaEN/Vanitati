   M S B A M L   `   `   `  ���� O  KUnnamedSstrategyGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:UnnamedStrategyGame.UIUnnamedStrategyGame.UI  $    UnnamedStrategyGame.UI.NewGame   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6local$clr-namespace:UnnamedStrategyGame.UI       N� Title$  NewGame�� N� 
Background �	#7F000000.����  �� Padding$ 10q�$��Center=�$��Center=�
�������._��� .+��  � ColumnDefinitions �� $
��Auto ��� $
��Auto � � RowDefinitions �� $
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
��Auto ��� $��* �.+��� $��Player Settings��$��Black"�$��16(�$��0��$��2��$��0���� $��Display Name��$��1���� -    ��Name$ optName��$��1��$��1��$��Center=��� $��Hosting Settings��$��Black"�$��16(�$��0��$��2��$��10���� $
��Mode��$��11��$
��Bold"��� $��1��$��11��.+�	� -    	�Name$ optLocalGame�� d� 	IsChecked$
 TrueY�.��
Local Game	� -   $ optNetworkedGame��.��Networked Game�� $
��Port��$��12���� -   $ optPort��$��1��$��12��$��Center=��� $��Battle Settings��$��Black"�$��16(�$��0��$��2��$��20���� $��Initial State��$��21��$
��Bold"��� -   $��1��$��21��$��Center=�.��Select�� $��22��.��Max Commanders�� -   $ nooptMaxCommanders��$��1��$��22��{�.  �� $��2��$��23��$
��Bold"�.��Victory Conditions�� -    ��Name$# optEnableVictoryPointsMaximum��$��2��$��24��.��Victory Points Maximum�� $��25��.��
Maximum VP�� -   $ optVictoryPointsMaximum��$��1��$��25��{��� 	 �� ElementName$#	 optEnableVictoryPointsMaximum��
 �� Path$
 	IsChecked��� -	   $ optEnableVictoryPointsGap��$��2��$��26��.��Victory Points Gap�� $��27��.��Maximum VP Gap�� -
   $ optVictoryPointsGap��$��1��$��27��{��� $	 optEnableVictoryPointsGap��$
 	IsChecked��� -    ��Name$ msgNoStateSelected��$��2��$��22��$��6��
�������$��Center=�$��Center=�.��(No State Selected)]� ��.  .+��� -   $��Right=�.��Cancel�� -    ��Name$ btnStartGame��$��Right=�.��
Start Game