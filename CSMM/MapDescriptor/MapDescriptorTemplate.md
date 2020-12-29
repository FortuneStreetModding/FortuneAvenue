<#@ template language="C#" #>
<#@ output encoding="utf-8"#>
<#@ import namespace="CustomStreetManager" #>
<#@ import namespace="FSEditor.FSData" #>
<#@ import namespace="System.Globalization" #>

# <#= md.Name[Locale.EN] #>

<#= md.Desc[Locale.EN] #>

## Screenshots

<Placeholder for screenshots>

## Features

### Map Properties

| Map Properties    |      Value |
| ----------------- | ---------- |
| Initial Cash      | <#= String.Format("{0,10}", md.InitialCash) #> |
| Target Amount     | <#= String.Format("{0,10}", md.TargetAmount) #> |
| Base Salary       | <#= String.Format("{0,10}", md.BaseSalary) #> |
| Salary Increment  | <#= String.Format("{0,10}", md.SalaryIncrement) #> |
| Maximum Dice Roll | <#= String.Format("{0,10}", md.MaxDiceRoll) #> |
| Looping Mode      | <#= String.Format("{0,10}", md.LoopingMode) #> |

### Map Configuration

<details>
  <summary>Click to expand!</summary>

| Map Configuration |                Value |
| ----------------- | -------------------- |
| Rules             | <#= String.Format("{0,20}", md.RuleSet) #> |
| Theme             | <#= String.Format("{0,20}", md.Theme) #> |
| FRB File Name 1   | <#= String.Format("{0,20}", md.FrbFile1) #> |
| FRB File Name 2   | <#= String.Format("{0,20}", md.FrbFile2) #> |
| FRB File Name 3   | <#= String.Format("{0,20}", md.FrbFile3) #> |
| FRB File Name 4   | <#= String.Format("{0,20}", md.FrbFile4) #> |

</details>

### Map Background

<details>
  <summary>Click to expand!</summary>

| On  | Background | Description          |
| --- | ---------- | -------------------- |
| <#= md.Background == "bg101"?  ":o:":"   " #> | bg101      | Trodain Castle       |
| <#= md.Background == "bg109"?  ":o:":"   " #> | bg109      | The Observatory      |
| <#= md.Background == "bg102"?  ":o:":"   " #> | bg102      | Ghost Ship           |
| <#= md.Background == "bg105"?  ":o:":"   " #> | bg105      | Slimenia             |
| <#= md.Background == "bg104"?  ":o:":"   " #> | bg104      | Mt. Magmageddon      |
| <#= md.Background == "bg106"?  ":o:":"   " #> | bg106      | Robbin' Hood Ruins   |
| <#= md.Background == "bg004"?  ":o:":"   " #> | bg004      | Mario Stadium        |
| <#= md.Background == "bg008"?  ":o:":"   " #> | bg008      | Starship Mario       |
| <#= md.Background == "bg002"?  ":o:":"   " #> | bg002      | Mario Circuit        |
| <#= md.Background == "bg001"?  ":o:":"   " #> | bg001      | Yoshi's Island       |
| <#= md.Background == "bg005"?  ":o:":"   " #> | bg005      | Delfino Plaza        |
| <#= md.Background == "bg003"?  ":o:":"   " #> | bg003      | Peach's Castle       |
| <#= md.Background == "bg107"?  ":o:":"   " #> | bg107      | Alefgard             |
| <#= md.Background == "bg006"?  ":o:":"   " #> | bg006      | Super Mario Bros     |
| <#= md.Background == "bg007"?  ":o:":"   " #> | bg007      | Bowser's Castle      |
| <#= md.Background == "bg009"?  ":o:":"   " #> | bg009      | Good Egg Galaxy      |
| <#= md.Background == "bg103"?  ":o:":"   " #> | bg103      | The Colossus         |
| <#= md.Background == "bg103_e"?":o:":"   " #> | bg103_e    | The Colossus Easy    |
| <#= md.Background == "bg108"?  ":o:":"   " #> | bg108      | Alltrades Abbey      |
| <#= md.Background == "bg901"?  ":o:":"   " #> | bg901      | Practice Board       |

</details>

### Map Icon

<details>
  <summary>Click to expand!</summary>

| On  | Icon       | Description          |
| --- | ---------- | -------------------- |
| <#= md.MapIcon == "p_bg_101"?":o:":"   " #> | p_bg_101   | Trodain Castle       |
| <#= md.MapIcon == "p_bg_109"?":o:":"   " #> | p_bg_109   | The Observatory      |
| <#= md.MapIcon == "p_bg_102"?":o:":"   " #> | p_bg_102   | Ghost Ship           |
| <#= md.MapIcon == "p_bg_105"?":o:":"   " #> | p_bg_105   | Slimenia             |
| <#= md.MapIcon == "p_bg_104"?":o:":"   " #> | p_bg_104   | Mt. Magmageddon      |
| <#= md.MapIcon == "p_bg_106"?":o:":"   " #> | p_bg_106   | Robbin' Hood Ruins   |
| <#= md.MapIcon == "p_bg_004"?":o:":"   " #> | p_bg_004   | Mario Stadium        |
| <#= md.MapIcon == "p_bg_008"?":o:":"   " #> | p_bg_008   | Starship Mario       |
| <#= md.MapIcon == "p_bg_002"?":o:":"   " #> | p_bg_002   | Mario Circuit        |
| <#= md.MapIcon == "p_bg_001"?":o:":"   " #> | p_bg_001   | Yoshi's Island       |
| <#= md.MapIcon == "p_bg_005"?":o:":"   " #> | p_bg_005   | Delfino Plaza        |
| <#= md.MapIcon == "p_bg_003"?":o:":"   " #> | p_bg_003   | Peach's Castle       |
| <#= md.MapIcon == "p_bg_107"?":o:":"   " #> | p_bg_107   | Alefgard             |
| <#= md.MapIcon == "p_bg_006"?":o:":"   " #> | p_bg_006   | Super Mario Bros     |
| <#= md.MapIcon == "p_bg_007"?":o:":"   " #> | p_bg_007   | Bowser's Castle      |
| <#= md.MapIcon == "p_bg_009"?":o:":"   " #> | p_bg_009   | Good Egg Galaxy      |
| <#= md.MapIcon == "p_bg_103"?":o:":"   " #> | p_bg_103   | The Colossus         |
| <#= md.MapIcon == "p_bg_108"?":o:":"   " #> | p_bg_108   | Alltrades Abbey      |

</details>

### Map Background Music

<details>
  <summary>Click to expand!</summary>

| On  | BGM | Brsar | Filename                 | Description              |
| --- | --- | ----- | ------------------------ | ------------------------ |
| <#= md.BGMID == 17?":o:":"   " #> |  17 |    29 | 29_BGM_MAP_TRODAIN       | Trodain Castle           |
| <#= md.BGMID == 21?":o:":"   " #> |  21 |    41 | 37_BGM_MAP_ANGEL         | The Observatory          |
| <#= md.BGMID ==  3?":o:":"   " #> |   3 |    31 | 30_BGM_MAP_GHOSTSHIP     | Ghost Ship               |
| <#= md.BGMID ==  6?":o:":"   " #> |   6 |    34 | 33_BGM_MAP_SLABACCA      | Slimenia                 |
| <#= md.BGMID ==  5?":o:":"   " #> |   5 |    33 | 32_BGM_MAP_SINOKAZAN     | Mt. Magmageddon          |
| <#= md.BGMID ==  7?":o:":"   " #> |   7 |    35 | 34_BGM_MAP_KANDATA       | Robbin' Hood Ruins       |
| <#= md.BGMID == 12?":o:":"   " #> |  12 |    23 | 23_BGM_MAP_STADIUM       | Mario Stadium            |
| <#= md.BGMID == 15?":o:":"   " #> |  15 |    27 | 27_BGM_MAP_STARSHIP      | Starship Mario           |
| <#= md.BGMID ==  0?":o:":"   " #> |   0 |    21 | 21_BGM_MAP_CIRCUIT       | Mario Circuit            |
| <#= md.BGMID == 11?":o:":"   " #> |  11 |    20 | 20_BGM_MAP_YOSHI         | Yoshi's Island           |
| <#= md.BGMID == 13?":o:":"   " #> |  13 |    24 | 24_BGM_MAP_DOLPIC        | Delfino Plaza            |
| <#= md.BGMID ==  1?":o:":"   " #> |   1 |    22 | 22_BGM_MAP_PEACH         | Peach's Castle           |
| <#= md.BGMID ==  9?":o:":"   " #> |   9 |    37 | 35_BGM_MAP_ALEFGARD      | Alefgard                 |
| <#= md.BGMID == 14?":o:":"   " #> |  14 |    25 | 25_BGM_MAP_SMB           | Super Mario Bros         |
| <#= md.BGMID ==  2?":o:":"   " #> |   2 |    26 | 26_BGM_MAP_KOOPA         | Bowser's Castle          |
| <#= md.BGMID == 16?":o:":"   " #> |  16 |    28 | 28_BGM_MAP_EGG           | Good Egg Galaxy          |
| <#= md.BGMID ==  4?":o:":"   " #> |   4 |    32 | 31_BGM_MAP_MAJINZOU      | The Colossus             |
| <#= md.BGMID == 19?":o:":"   " #> |  19 |    39 | 36_BGM_MAP_DHAMA         | Alltrades Abbey          |
| <#= md.BGMID == 22?":o:":"   " #> |  22 |     5 | 05_BGM_MENU              | Practice Board           |
| <#= md.BGMID ==  8?":o:":"   " #> |   8 |    36 | 34_BGM_MAP_KANDATA_old   | Unused                   |
| <#= md.BGMID == 10?":o:":"   " #> |  10 |    38 | 35_BGM_MAP_ALEFGARD_old  | Unused                   |
| <#= md.BGMID == 18?":o:":"   " #> |  18 |    30 | 29_BGM_MAP_TRODAIN_old   | Unused                   |
| <#= md.BGMID == 20?":o:":"   " #> |  20 |    40 | 36_BGM_MAP_DHAMA_old     | Unused                   |
| <#= md.BGMID == 23?":o:":"   " #> |  23 | 42/43 | 38_BGM_GOALPROP_(M/D)    | Promotion                |
| <#= md.BGMID == 24?":o:":"   " #> |  24 | 10/11 | 10_BGM_WINNER_(M/D)      | Winner                   |
| <#= md.BGMID == 25?":o:":"   " #> |  25 |    12 | 12_BGM_CHANCECARD        | Select Chancecard        |
| <#= md.BGMID == 26?":o:":"   " #> |  26 |    13 | 13_BGM_STOCK             | Buy/Sell Stock           |
| <#= md.BGMID == 27?":o:":"   " #> |  27 |    14 | 14_BGM_AUCTION           | Auction                  |
| <#= md.BGMID == 28?":o:":"   " #> |  28 | 15/16 | 15_BGM_CASINO_SLOT_(M/D) | Round The Blocks         |
| <#= md.BGMID == 29?":o:":"   " #> |  29 |    17 | 15_BGM_CASINO_BLOCK      | Memory Block             |
| <#= md.BGMID == 30?":o:":"   " #> |  30 |    12 | 12_BGM_CHANCECARD        | Dart of Gold             |
| <#= md.BGMID == 31?":o:":"   " #> |  31 |    16 | 16_BGM_CASINO_SLOT_D     | Select your Slime        |
| <#= md.BGMID == 32?":o:":"   " #> |  32 |    19 | 19_BGM_CASINO_RACE       | Racing Slimes            |
| <#= md.BGMID == 33?":o:":"   " #> |  33 |     0 | 01_BGM_TITLE             | Title Screen             |
| <#= md.BGMID == 34?":o:":"   " #> |  34 |     5 | 05_BGM_MENU              | Menu                     |
| <#= md.BGMID == 35?":o:":"   " #> |  35 |     3 | 04_BGM_SAVELOAD          | Save/Load Screen         |
| <#= md.BGMID == 36?":o:":"   " #> |  36 |     4 | 04_BGM_SAVELOAD_old      | Unused                   |
| <#= md.BGMID == 37?":o:":"   " #> |  37 |     6 | 06_BGM_WIFI              | Wi-Fi                    |
| <#= md.BGMID == 38?":o:":"   " #> |  38 |     3 | 04_BGM_SAVELOAD          | Unknown                  |
| <#= md.BGMID == 39?":o:":"   " #> |  39 |     7 | 07_BGM_ENDING_M          | Credits                  |

</details>
<# if(md.SwitchRotationOriginPoints.Count > 0) { #>
### Switch Rotation Animation Configuration

<details>
  <summary>Click to expand!</summary>

| Switch Rotation Origin Points |           Value |
| ----------------------------- | --------------- |
<#     for(int i = 0; i < md.SwitchRotationOriginPoints.Count; i++) { 
#><#=      String.Format("| {0,-29} | {1,15} |", "Rotation Origin Point "+ (i+1) +" X", md.SwitchRotationOriginPoints[i].X.ToString(CultureInfo.InvariantCulture)) #>
<#=        String.Format("| {0,-29} | {1,15} |", "Rotation Origin Point "+ (i+1) +" Y", md.SwitchRotationOriginPoints[i].Y.ToString(CultureInfo.InvariantCulture)) #>
<#     } #>
</details>
<# } if(md.LoopingMode != LoopingMode.None) { #>
### Looping Mode Configuration

<details>
  <summary>Click to expand!</summary>

| Looping Mode Configuration    |           Value |
| ----------------------------- | --------------- |
| Radius                        | <#= String.Format("{0,15}", md.LoopingModeRadius.ToString(CultureInfo.InvariantCulture)) #> |
| Horizontal Padding            | <#= String.Format("{0,15}", md.LoopingModeHorizontalPadding.ToString(CultureInfo.InvariantCulture)) #> |
| Vertical Square Count         | <#= String.Format("{0,15}", md.LoopingModeVerticalSquareCount.ToString(CultureInfo.InvariantCulture)) #> |

</details>
<# } #>
### Tour Configuration

<details>
  <summary>Click to expand!</summary>

| Tour Configuration     |           Value |
| ---------------------- | --------------- |
| Tour Bankruptcy Limit  | <#= String.Format("{0,15}", md.TourBankruptcyLimit) #> |
| Tour Initial Cash      | <#= String.Format("{0,15}", md.TourInitialCash) #> |
| Tour Opponent 1        | <#= String.Format("{0,15}", md.TourOpponent1) #> |
| Tour Opponent 2        | <#= String.Format("{0,15}", md.TourOpponent2) #> |
| Tour Opponent 3        | <#= String.Format("{0,15}", md.TourOpponent3) #> |
| Tour Clear Rank        | <#= String.Format("{0,15}", md.TourClearRank) #> |

</details>

### Localization

<details>
  <summary>Click to expand!</summary>
    
| Message   | String |
| --------- | ------ |
| Name (DE) | <#= md.Name[Locale.DE] #> |
| Name (ES) | <#= md.Name[Locale.ES] #> |
| Name (FR) | <#= md.Name[Locale.FR] #> |
| Name (IT) | <#= md.Name[Locale.IT] #> |
| Name (JP) | <#= md.Name[Locale.JP] #> |
| Desc (DE) | <#= md.Desc[Locale.DE] #> |
| Desc (ES) | <#= md.Desc[Locale.ES] #> |
| Desc (FR) | <#= md.Desc[Locale.FR] #> |
| Desc (IT) | <#= md.Desc[Locale.IT] #> |
| Desc (JP) | <#= md.Desc[Locale.JP] #> |

</details>

### Venture Cards

<details>
  <summary>Click to expand!</summary>

| ID  | On  | Description                                                                                                      |
| --- | --- | ---------------------------------------------------------------------------------------------------------------- |
|   1 | <#= md.VentureCard[  0] != 0?":o:":"   " #> | Adventurous turning point! You can choose which way to move on your next go, (player's name).                    |
|   2 | <#= md.VentureCard[  1] != 0?":o:":"   " #> | Venture on! Roll the die again and move forward.                                                                 |
|   3 | <#= md.VentureCard[  2] != 0?":o:":"   " #> | Venture through space! Zoom over to any non-venture, non-suit square you like!                                   |
|   4 | <#= md.VentureCard[  3] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 40 times the number shown in gold coins from the player in 1st place!  |
|   5 | <#= md.VentureCard[  4] != 0?":o:":"   " #> | Venture through space! Zoom over to any shop or vacant plot!                                                     |
|   6 | <#= md.VentureCard[  5] != 0?":o:":"   " #> | Venture through space! Zoom over to any venture or suit square!                                                  |
|   7 | <#= md.VentureCard[  6] != 0?":o:":"   " #> | Special bonus! Your shops all grow by 7%!                                                                        |
|   8 | <#= md.VentureCard[  7] != 0?":o:":"   " #> | Venture on! Everyone's shop prices increase by 30%! Now roll the die and move again.                             |
|   9 | <#= md.VentureCard[  8] != 0?":o:":"   " #> | Venture on! Everyone's shops close for the day! Now roll the die and move again.                                 |
|  10 | <#= md.VentureCard[  9] != 0?":o:":"   " #> | Venture on! Everyone's shop prices cut in half! Now roll the die and move again.                                 |
|  11 | <#= md.VentureCard[ 10] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 11 times the number shown in gold coins from all other players!        |
|  12 | <#= md.VentureCard[ 11] != 0?":o:":"   " #> | Capital venture! You can invest capital in any of your shops.                                                    |
|  13 | <#= md.VentureCard[ 12] != 0?":o:":"   " #> | Misadventure! The values of all your shops drop by 13%!                                                          |
|  14 | <#= md.VentureCard[ 13] != 0?":o:":"   " #> | Misadventure! You give everyone 30G each!                                                                        |
|  15 | <#= md.VentureCard[ 14] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 50 times the number shown in gold coins from the bank!                 |
|  16 | <#= md.VentureCard[ 15] != 0?":o:":"   " #> | Random venture! Shops expand in three districts picked at random!                                                |
|  17 | <#= md.VentureCard[ 16] != 0?":o:":"   " #> | Special bonus! You receive half of your salary!                                                                  |
|  18 | <#= md.VentureCard[ 17] != 0?":o:":"   " #> | Misadventure! The bank is forcibly buying you out! You're compelled to sell a shop for only twice its value.     |
|  19 | <#= md.VentureCard[ 18] != 0?":o:":"   " #> | Price hike venture! Your shop prices go up by 30% until your next turn.                                          |
|  20 | <#= md.VentureCard[ 19] != 0?":o:":"   " #> | Revaluation venture! You can expand any one of your shops by 20%.                                                |
|  21 | <#= md.VentureCard[ 20] != 0?":o:":"   " #> | Random venture! You receive 20 stocks in a district picked at random!                                            |
|  22 | <#= md.VentureCard[ 21] != 0?":o:":"   " #> | Cashback venture! You can sell a shop back to the bank for twice its shop value.                                 |
|  23 | <#= md.VentureCard[ 22] != 0?":o:":"   " #> | Revaluation venture! You can expand any one of your shops by 50%.                                                |
|  24 | <#= md.VentureCard[ 23] != 0?":o:":"   " #> | Misadventure! The bank is forcibly buying you out! You're compelled to sell a shop for 200G more than its value. |
|  25 | <#= md.VentureCard[ 24] != 0?":o:":"   " #> | Misadventure! Your shop prices halve until your next turn!                                                       |
|  26 | <#= md.VentureCard[ 25] != 0?":o:":"   " #> | Lucky venture! You get a big commission until your next turn!                                                    |
|  27 | <#= md.VentureCard[ 26] != 0?":o:":"   " #> | Special bonus! You receive 27 times the number of shops you own in gold coins from the bank!                     |
|  28 | <#= md.VentureCard[ 27] != 0?":o:":"   " #> | Cameo adventure! A goodybag appears!                                                                             |
|  29 | <#= md.VentureCard[ 28] != 0?":o:":"   " #> | Freebie! Take a Heart!                                                                                           |
|  30 | <#= md.VentureCard[ 29] != 0?":o:":"   " #> | Venture on! All shops charge a 100G flat rate! Now roll the die and move again.                                  |
|  31 | <#= md.VentureCard[ 30] != 0?":o:":"   " #> | Random venture! Shops expand by 10% in a district picked at random!                                              |
|  32 | <#= md.VentureCard[ 31] != 0?":o:":"   " #> | Random venture! Shops expand by 20% in a district picked at random!                                              |
|  33 | <#= md.VentureCard[ 32] != 0?":o:":"   " #> | Cashback venture! You can sell a shop back to the bank for three times its shop value.                           |
|  34 | <#= md.VentureCard[ 33] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 and your shops close for the day. Roll 2/4/6 and everyone else's shops close.        |
|  35 | <#= md.VentureCard[ 34] != 0?":o:":"   " #> | Stock venture! You can sell stocks you own at 35% above the market value.                                        |
|  36 | <#= md.VentureCard[ 35] != 0?":o:":"   " #> | Capital venture! You can pay 100G for the chance to invest in your shops.                                        |
|  37 | <#= md.VentureCard[ 36] != 0?":o:":"   " #> | Random venture! Shops expand by 30% in a district picked at random!                                              |
|  38 | <#= md.VentureCard[ 37] != 0?":o:":"   " #> | Stock venture! You can buy stocks in a district of your choice at 10% above the market value.                    |
|  39 | <#= md.VentureCard[ 38] != 0?":o:":"   " #> | Suit venture! Buy a Suit Yourself card for 100G.                                                                 |
|  40 | <#= md.VentureCard[ 39] != 0?":o:":"   " #> | Misadventure! You give away 10% of your ready cash to the player in last place!                                  |
|  41 | <#= md.VentureCard[ 40] != 0?":o:":"   " #> | Misadventure! Stock prices fall by 10% in a district picked at random!                                           |
|  42 | <#= md.VentureCard[ 41] != 0?":o:":"   " #> | Misadventure! Stock prices fall by 20% in a district picked at random!                                           |
|  43 | <#= md.VentureCard[ 42] != 0?":o:":"   " #> | Misadventure! You pay an assets tax of two gold coins per unit of stock that you own!                            |
|  44 | <#= md.VentureCard[ 43] != 0?":o:":"   " #> | Misadventure! Roll the die and pay 44 times the number in gold coins to the player in last place!                |
|  45 | <#= md.VentureCard[ 44] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 to warp to a take-a-break square. Roll 2/4/6 to warp to the arcade.                  |
|  46 | <#= md.VentureCard[ 45] != 0?":o:":"   " #> | Misadventure! You drop your wallet and lose 10% of your ready cash!                                              |
|  47 | <#= md.VentureCard[ 46] != 0?":o:":"   " #> | Dicey adventure! Roll 2-6 to get all the suits. Roll 1 and lose all your suits.                                  |
|  48 | <#= md.VentureCard[ 47] != 0?":o:":"   " #> | Misadventure! All shops in a district picked at random fall in value by 10%!                                     |
|  49 | <#= md.VentureCard[ 48] != 0?":o:":"   " #> | Misadventure! All shops in a district picked at random fall in value by 20%!                                     |
|  50 | <#= md.VentureCard[ 49] != 0?":o:":"   " #> | Venture on! Move forward the same number of squares again.                                                       |
|  51 | <#= md.VentureCard[ 50] != 0?":o:":"   " #> | Venture on! Move forward 1 square more.                                                                          |
|  52 | <#= md.VentureCard[ 51] != 0?":o:":"   " #> | Venture on! Move forward another 2 squares.                                                                      |
|  53 | <#= md.VentureCard[ 52] != 0?":o:":"   " #> | Venture through space! Zoom over to the bank!                                                                    |
|  54 | <#= md.VentureCard[ 53] != 0?":o:":"   " #> | Venture through space! Pay 100G to zoom straight to the bank!                                                    |
|  55 | <#= md.VentureCard[ 54] != 0?":o:":"   " #> | Venture on! Roll the die again and move forward (with an invitation to browse thrown in!).                       |
|  56 | <#= md.VentureCard[ 55] != 0?":o:":"   " #> | Venture on! Roll the die again and move forward (with a half-price special offer thrown in!).                    |
|  57 | <#= md.VentureCard[ 56] != 0?":o:":"   " #> | Venture through space! Zoom to any square you like.                                                              |
|  58 | <#= md.VentureCard[ 57] != 0?":o:":"   " #> | Venture through space! Pay 100G to zoom to any non-venture, non-suit square you like!                            |
|  59 | <#= md.VentureCard[ 58] != 0?":o:":"   " #> | Stock venture! You can buy stocks in a district of your choice at 10% below the market value.                    |
|  60 | <#= md.VentureCard[ 59] != 0?":o:":"   " #> | Random venture! Stock prices increase by 10% in a district picked at random!                                     |
|  61 | <#= md.VentureCard[ 60] != 0?":o:":"   " #> | Special bonus! You receive a 10% dividend on your stocks!                                                        |
|  62 | <#= md.VentureCard[ 61] != 0?":o:":"   " #> | Special bonus! You receive a 20% dividend on your stocks!                                                        |
|  63 | <#= md.VentureCard[ 62] != 0?":o:":"   " #> | Random venture! Stock prices increase by 20% in a district picked at random!                                     |
|  64 | <#= md.VentureCard[ 63] != 0?":o:":"   " #> | Random venture! Stock prices increase by 30% in a district picked at random!                                     |
|  65 | <#= md.VentureCard[ 64] != 0?":o:":"   " #> | Forced buyout! You can buy a vacant plot or shop for five times its value, whether someone else owns it or not.  |
|  66 | <#= md.VentureCard[ 65] != 0?":o:":"   " #> | Special bonus! You receive 10 of the most valuable stocks!                                                       |
|  67 | <#= md.VentureCard[ 66] != 0?":o:":"   " #> | Stock venture! You can buy stocks in a district of your choice.                                                  |
|  68 | <#= md.VentureCard[ 67] != 0?":o:":"   " #> | Special arcade adventure! You're invited to play Memory Block!                                                   |
|  69 | <#= md.VentureCard[ 68] != 0?":o:":"   " #> | Stock venture! You can sell stocks you own at 20% above the market value.                                        |
|  70 | <#= md.VentureCard[ 69] != 0?":o:":"   " #> | Special bonus! You get a sudden promotion and receive a salary! (You lose any suits you have.)                   |
|  71 | <#= md.VentureCard[ 70] != 0?":o:":"   " #> | Capital venture! You can invest up to 200G of the bank's money in your shops.                                    |
|  72 | <#= md.VentureCard[ 71] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 to take 20 times the number of your shops in gold coins. Roll 2/4/6 to pay the same. |
|  73 | <#= md.VentureCard[ 72] != 0?":o:":"   " #> | Property venture! You can buy any unowned shop or vacant plot.                                                   |
|  74 | <#= md.VentureCard[ 73] != 0?":o:":"   " #> | Misadventure! You are forced to auction one of your shops (with a starting price of twice the shop's value).     |
|  75 | <#= md.VentureCard[ 74] != 0?":o:":"   " #> | Property venture! You can buy any unowned shop or vacant plot for twice its value.                               |
|  76 | <#= md.VentureCard[ 75] != 0?":o:":"   " #> | Special arcade adventure! You're invited to play Round the Blocks!                                               |
|  77 | <#= md.VentureCard[ 76] != 0?":o:":"   " #> | Freebie! Take five of each district's stocks.                                                                    |
|  78 | <#= md.VentureCard[ 77] != 0?":o:":"   " #> | Property venture! You can buy any unowned shop or vacant plot for 200G more than its value.                      |
|  79 | <#= md.VentureCard[ 78] != 0?":o:":"   " #> | Forced buyout! You can buy a vacant plot or shop for three times its value, whether someone else owns it or not. |
|  80 | <#= md.VentureCard[ 79] != 0?":o:":"   " #> | Freebie! Take a Spade!                                                                                           |
|  81 | <#= md.VentureCard[ 80] != 0?":o:":"   " #> | Misadventure! All other players can only move forward 1 on their next turn.                                      |
|  82 | <#= md.VentureCard[ 81] != 0?":o:":"   " #> | Freebie! Take a Club!                                                                                            |
|  83 | <#= md.VentureCard[ 82] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 and warp to a random location. Roll 2/4/6 and everyone else warps.                   |
|  84 | <#= md.VentureCard[ 83] != 0?":o:":"   " #> | Moneymaking venture! The winning player must pay you 10% of their ready cash!                                    |
|  85 | <#= md.VentureCard[ 84] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 85 times the number shown in gold coins from the bank!                 |
|  86 | <#= md.VentureCard[ 85] != 0?":o:":"   " #> | Moneymaking venture! Take 100G from all other players!                                                           |
|  87 | <#= md.VentureCard[ 86] != 0?":o:":"   " #> | Venture on! Roll the special all-7s-and-8s die and move forward again.                                           |
|  88 | <#= md.VentureCard[ 87] != 0?":o:":"   " #> | Misadventure! All other players swap places!                                                                     |
|  89 | <#= md.VentureCard[ 88] != 0?":o:":"   " #> | Freebie! All players take a Suit Yourself card!                                                                  |
|  90 | <#= md.VentureCard[ 89] != 0?":o:":"   " #> | Price hike venture! All shop prices go up by 30% until your next turn.                                           |
|  91 | <#= md.VentureCard[ 90] != 0?":o:":"   " #> | Cameo adventure! A healslime appears!                                                                            |
|  92 | <#= md.VentureCard[ 91] != 0?":o:":"   " #> | Cameo adventure! Lakitu appears!                                                                                 |
|  93 | <#= md.VentureCard[ 92] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 and your shops expand by 10%. Roll 2/4/6 and everyone else's shops expand by 5%.     |
|  94 | <#= md.VentureCard[ 93] != 0?":o:":"   " #> | Freebie! Take a Diamond!                                                                                         |
|  95 | <#= md.VentureCard[ 94] != 0?":o:":"   " #> | Misadventure! You throw an impromptu party. All other players come to your location!                             |
|  96 | <#= md.VentureCard[ 95] != 0?":o:":"   " #> | Misadventure! All players scramble to another player's location!                                                 |
|  97 | <#= md.VentureCard[ 96] != 0?":o:":"   " #> | Stock rise venture! Increase stock value by 20% in a district of your choice.                                    |
|  98 | <#= md.VentureCard[ 97] != 0?":o:":"   " #> | Forced buyout! You can buy a vacant plot or shop for four times its value, whether someone else owns it or not.  |
|  99 | <#= md.VentureCard[ 98] != 0?":o:":"   " #> | Freebie! What's inside...?                                                                                       |
| 100 | <#= md.VentureCard[ 99] != 0?":o:":"   " #> | Freebie! Take a Suit Yourself card!                                                                              |
| 101 | <#= md.VentureCard[100] != 0?":o:":"   " #> | Special bonus! Your shops all grow by 21%!                                                                       |
| 102 | <#= md.VentureCard[101] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 33 times the number shown in gold coins from all other players!        |
| 103 | <#= md.VentureCard[102] != 0?":o:":"   " #> | Misadventure! The values of all your shops drop by 25%!                                                          |
| 104 | <#= md.VentureCard[103] != 0?":o:":"   " #> | Misadventure! You give everyone 80G each!                                                                        |
| 105 | <#= md.VentureCard[104] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get the number shown x your level x 40G from the bank!                     |
| 106 | <#= md.VentureCard[105] != 0?":o:":"   " #> | Freebie! Roll the die and get half the number shown of Suit Yourself cards! (Decimals will be rounded down.)     |
| 107 | <#= md.VentureCard[106] != 0?":o:":"   " #> | Revaluation venture! You can expand any one of your shops by 30%.                                                |
| 108 | <#= md.VentureCard[107] != 0?":o:":"   " #> | Cashback venture! You can sell a shop back to the bank for four times its shop value.                            |
| 109 | <#= md.VentureCard[108] != 0?":o:":"   " #> | Revaluation venture! You can expand any one of your shops by 75%.                                                |
| 110 | <#= md.VentureCard[109] != 0?":o:":"   " #> | Special bonus! You receive 77 times the number of shops you own in gold coins from the bank!                     |
| 111 | <#= md.VentureCard[110] != 0?":o:":"   " #> | Cashback venture! You can sell a shop back to the bank for 500G more than its shop value.                        |
| 112 | <#= md.VentureCard[111] != 0?":o:":"   " #> | Special bonus! You receive 100 times the number of shops you own in gold coins!                                  |
| 113 | <#= md.VentureCard[112] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get the number shown x your level x 20G from the bank!                     |
| 114 | <#= md.VentureCard[113] != 0?":o:":"   " #> | Moneymaking venture! Take your level times 40G from all other players!                                           |
| 115 | <#= md.VentureCard[114] != 0?":o:":"   " #> | Misadventure! All other players can only move forward 7 on their next turn.                                      |
| 116 | <#= md.VentureCard[115] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 60 times the number shown in gold coins from the player in 1st place!  |
| 117 | <#= md.VentureCard[116] != 0?":o:":"   " #> | Adventurous turning point! Everyone gets to choose which way to move on their next go.                           |
| 118 | <#= md.VentureCard[117] != 0?":o:":"   " #> | Lucky venture! You get a really big commission until your next turn!                                             |
| 119 | <#= md.VentureCard[118] != 0?":o:":"   " #> | Misadventure! You give 20% of your ready cash to the player in last place!                                       |
| 120 | <#= md.VentureCard[119] != 0?":o:":"   " #> | Misadventure! You drop your wallet and lose 20% of your ready cash!                                              |
| 121 | <#= md.VentureCard[120] != 0?":o:":"   " #> | Capital venture! You can invest up to 400G of the bank's money in your shops.                                    |
| 122 | <#= md.VentureCard[121] != 0?":o:":"   " #> | Moneymaking venture! The winning player must pay you 20% of their ready cash!                                    |
| 123 | <#= md.VentureCard[122] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 and your shops expand by 20%. Roll 2/4/6 and everyone else's shops expand by 5%.     |
| 124 | <#= md.VentureCard[123] != 0?":o:":"   " #> | Suit venture! Buy a Suit Yourself card for 50G.                                                                  |
| 125 | <#= md.VentureCard[124] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 to warp to a boon square. Roll 2/4/6 to warp to the arcade.                          |
| 126 | <#= md.VentureCard[125] != 0?":o:":"   " #> | Revaluation venture! Roll the die and expand your shops by 2% for each number.                                   |
| 127 | <#= md.VentureCard[126] != 0?":o:":"   " #> | Special arcade adventure! You're invited to play Round the Blocks and Memory Block!                              |
| 128 | <#= md.VentureCard[127] != 0?":o:":"   " #> | Special bonus! You receive 55 times the number of shops you own in gold coins from the bank!                     |

</details>

## Prerequisites

- <Placeholder for prerequisites>

## Changelog

### v1
- <Placeholder for version information>

## Authors

- <Placeholder for author information>