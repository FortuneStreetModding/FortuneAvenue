<#@ template language="C#" #>
<#@ output encoding="utf-8"#>

# <#= md.Name_EN #>

<#= md.Desc_EN #>

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
| <#= md.Background == "bg101"?":o:":"   " #> | bg101      | Trodain Castle       |
| <#= md.Background == "bg109"?":o:":"   " #> | bg109      | The Observatory      |
| <#= md.Background == "bg102"?":o:":"   " #> | bg102      | Ghost Ship           |
| <#= md.Background == "bg105"?":o:":"   " #> | bg105      | Slimenia             |
| <#= md.Background == "bg104"?":o:":"   " #> | bg104      | Mt. Magmageddon      |
| <#= md.Background == "bg106"?":o:":"   " #> | bg106      | Robbin' Hood Ruins   |
| <#= md.Background == "bg004"?":o:":"   " #> | bg004      | Mario Stadium        |
| <#= md.Background == "bg008"?":o:":"   " #> | bg008      | Starship Mario       |
| <#= md.Background == "bg002"?":o:":"   " #> | bg002      | Mario Circuit        |
| <#= md.Background == "bg001"?":o:":"   " #> | bg001      | Yoshi's Island       |
| <#= md.Background == "bg005"?":o:":"   " #> | bg005      | Delfino Plaza        |
| <#= md.Background == "bg003"?":o:":"   " #> | bg003      | Peach's Castle       |
| <#= md.Background == "bg107"?":o:":"   " #> | bg107      | Alefgard             |
| <#= md.Background == "bg006"?":o:":"   " #> | bg006      | Super Mario Bros     |
| <#= md.Background == "bg007"?":o:":"   " #> | bg007      | Bowser's Castle      |
| <#= md.Background == "bg009"?":o:":"   " #> | bg009      | Good Egg Galaxy      |
| <#= md.Background == "bg103"?":o:":"   " #> | bg103      | The Colossus         |
| <#= md.Background == "bg108"?":o:":"   " #> | bg108      | Alltrades Abbey      |
| <#= md.Background == "bg901"?":o:":"   " #> | bg901      | Practice Board       |

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
| <#= md.BGMID == 36?":o:":"   " #> |  36 |     4 | 04_BGM_SAVELOAD_old      | unused                   |
| <#= md.BGMID == 37?":o:":"   " #> |  37 |     6 | 06_BGM_WIFI              | Wi-Fi                    |
| <#= md.BGMID == 38?":o:":"   " #> |  38 |     3 | 04_BGM_SAVELOAD          | Unknown                  |
| <#= md.BGMID == 39?":o:":"   " #> |  39 |     7 | 07_BGM_ENDING_M          | Credits                  |

</details>

### Tour Configuration

<details>
  <summary>Click to expand!</summary>

| Tour Configuration     | Value           |
| ---------------------- | --------------- |
| Tour Bankruptcy Limit  | <#= String.Format("{0,15}", md.TourBankruptcyLimit) #> |
| Tour Initial Cash      | <#= String.Format("{0,15}", md.TourInitialCash) #> |
| Tour Opponent 1        | <#= String.Format("{0,15}", md.TourOpponent1) #> |
| Tour Opponent 2        | <#= String.Format("{0,15}", md.TourOpponent2) #> |
| Tour Opponent 3        | <#= String.Format("{0,15}", md.TourOpponent3) #> |
| Tour Clear Rank        | <#= String.Format("{0,15}", md.TourClearRank) #> |
| Tour Difficulty        | <#= String.Format("{0,15}", md.TourDifficulty) #> |
| Tour General Play Time | <#= String.Format("{0,15}", md.TourGeneralPlayTime) #> |

</details>

### Localization

<details>
  <summary>Click to expand!</summary>
    
| Message   | String |
| --------- | ------ |
| Name (DE) | <#= md.Name_DE #> |
| Name (ES) | <#= md.Name_SU #> |
| Name (FR) | <#= md.Name_FR #> |
| Name (IT) | <#= md.Name_IT #> |
| Name (JP) | <#= md.Name_JP #> |
| Desc (DE) | <#= md.Desc_DE #> |
| Desc (ES) | <#= md.Desc_SU #> |
| Desc (FR) | <#= md.Desc_DE #> |
| Desc (IT) | <#= md.Desc_SU #> |
| Desc (JP) | <#= md.Desc_FR #> |

</details>

### Venture Cards

<details>
  <summary>Click to expand!</summary>

| ID  | On  | Description                                                                                                      |
| --- | --- | ---------------------------------------------------------------------------------------------------------------- |
|   1 | <#= md.Venture_Cards[  0] != 0?":o:":"   " #> | Adventurous turning point! You can choose which way to move on your next go, (player's name).                    |
|   2 | <#= md.Venture_Cards[  1] != 0?":o:":"   " #> | Venture on! Roll the die again and move forward.                                                                 |
|   3 | <#= md.Venture_Cards[  2] != 0?":o:":"   " #> | Venture through space! Zoom over to any non-venture, non-suit square you like!                                   |
|   4 | <#= md.Venture_Cards[  3] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 40 times the number shown in gold coins from the player in 1st place!  |
|   5 | <#= md.Venture_Cards[  4] != 0?":o:":"   " #> | Venture through space! Zoom over to any shop or vacant plot!                                                     |
|   6 | <#= md.Venture_Cards[  5] != 0?":o:":"   " #> | Venture through space! Zoom over to any venture or suit square!                                                  |
|   7 | <#= md.Venture_Cards[  6] != 0?":o:":"   " #> | Special bonus! Your shops all grow by 7%!                                                                        |
|   8 | <#= md.Venture_Cards[  7] != 0?":o:":"   " #> | Venture on! Everyone's shop prices increase by 30%! Now roll the die and move again.                             |
|   9 | <#= md.Venture_Cards[  8] != 0?":o:":"   " #> | Venture on! Everyone's shops close for the day! Now roll the die and move again.                                 |
|  10 | <#= md.Venture_Cards[  9] != 0?":o:":"   " #> | Venture on! Everyone's shop prices cut in half! Now roll the die and move again.                                 |
|  11 | <#= md.Venture_Cards[ 10] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 11 times the number shown in gold coins from all other players!        |
|  12 | <#= md.Venture_Cards[ 11] != 0?":o:":"   " #> | Capital venture! You can invest capital in any of your shops.                                                    |
|  13 | <#= md.Venture_Cards[ 12] != 0?":o:":"   " #> | Misadventure! The values of all your shops drop by 13%!                                                          |
|  14 | <#= md.Venture_Cards[ 13] != 0?":o:":"   " #> | Misadventure! You give everyone 30G each!                                                                        |
|  15 | <#= md.Venture_Cards[ 14] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 50 times the number shown in gold coins from the bank!                 |
|  16 | <#= md.Venture_Cards[ 15] != 0?":o:":"   " #> | Random venture! Shops expand in three districts picked at random!                                                |
|  17 | <#= md.Venture_Cards[ 16] != 0?":o:":"   " #> | Special bonus! You receive half of your salary!                                                                  |
|  18 | <#= md.Venture_Cards[ 17] != 0?":o:":"   " #> | Misadventure! The bank is forcibly buying you out! You're compelled to sell a shop for only twice its value.     |
|  19 | <#= md.Venture_Cards[ 18] != 0?":o:":"   " #> | Price hike venture! Your shop prices go up by 30% until your next turn.                                          |
|  20 | <#= md.Venture_Cards[ 19] != 0?":o:":"   " #> | Revaluation venture! You can expand any one of your shops by 20%.                                                |
|  21 | <#= md.Venture_Cards[ 20] != 0?":o:":"   " #> | Random venture! You receive 20 stocks in a district picked at random!                                            |
|  22 | <#= md.Venture_Cards[ 21] != 0?":o:":"   " #> | Cashback venture! You can sell a shop back to the bank for twice its shop value.                                 |
|  23 | <#= md.Venture_Cards[ 22] != 0?":o:":"   " #> | Revaluation venture! You can expand any one of your shops by 50%.                                                |
|  24 | <#= md.Venture_Cards[ 23] != 0?":o:":"   " #> | Misadventure! The bank is forcibly buying you out! You're compelled to sell a shop for 200G more than its value. |
|  25 | <#= md.Venture_Cards[ 24] != 0?":o:":"   " #> | Misadventure! Your shop prices halve until your next turn!                                                       |
|  26 | <#= md.Venture_Cards[ 25] != 0?":o:":"   " #> | Lucky venture! You get a big commission until your next turn!                                                    |
|  27 | <#= md.Venture_Cards[ 26] != 0?":o:":"   " #> | Special bonus! You receive 27 times the number of shops you own in gold coins from the bank!                     |
|  28 | <#= md.Venture_Cards[ 27] != 0?":o:":"   " #> | Cameo adventure! A goodybag appears!                                                                             |
|  29 | <#= md.Venture_Cards[ 28] != 0?":o:":"   " #> | Freebie! Take a Heart!                                                                                           |
|  30 | <#= md.Venture_Cards[ 29] != 0?":o:":"   " #> | Venture on! All shops charge a 100G flat rate! Now roll the die and move again.                                  |
|  31 | <#= md.Venture_Cards[ 30] != 0?":o:":"   " #> | Random venture! Shops expand by 10% in a district picked at random!                                              |
|  32 | <#= md.Venture_Cards[ 31] != 0?":o:":"   " #> | Random venture! Shops expand by 20% in a district picked at random!                                              |
|  33 | <#= md.Venture_Cards[ 32] != 0?":o:":"   " #> | Cashback venture! You can sell a shop back to the bank for three times its shop value.                           |
|  34 | <#= md.Venture_Cards[ 33] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 and your shops close for the day. Roll 2/4/6 and everyone else's shops close.        |
|  35 | <#= md.Venture_Cards[ 34] != 0?":o:":"   " #> | Stock venture! You can sell stocks you own at 35% above the market value.                                        |
|  36 | <#= md.Venture_Cards[ 35] != 0?":o:":"   " #> | Capital venture! You can pay 100G for the chance to invest in your shops.                                        |
|  37 | <#= md.Venture_Cards[ 36] != 0?":o:":"   " #> | Random venture! Shops expand by 30% in a district picked at random!                                              |
|  38 | <#= md.Venture_Cards[ 37] != 0?":o:":"   " #> | Stock venture! You can buy stocks in a district of your choice at 10% above the market value.                    |
|  39 | <#= md.Venture_Cards[ 38] != 0?":o:":"   " #> | Suit venture! Buy a Suit Yourself card for 100G.                                                                 |
|  40 | <#= md.Venture_Cards[ 39] != 0?":o:":"   " #> | Misadventure! You give away 10% of your ready cash to the player in last place!                                  |
|  41 | <#= md.Venture_Cards[ 40] != 0?":o:":"   " #> | Misadventure! Stock prices fall by 10% in a district picked at random!                                           |
|  42 | <#= md.Venture_Cards[ 41] != 0?":o:":"   " #> | Misadventure! Stock prices fall by 20% in a district picked at random!                                           |
|  43 | <#= md.Venture_Cards[ 42] != 0?":o:":"   " #> | Misadventure! You pay an assets tax of two gold coins per unit of stock that you own!                            |
|  44 | <#= md.Venture_Cards[ 43] != 0?":o:":"   " #> | Misadventure! Roll the die and pay 44 times the number in gold coins to the player in last place!                |
|  45 | <#= md.Venture_Cards[ 44] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 to warp to a take-a-break square. Roll 2/4/6 to warp to the arcade.                  |
|  46 | <#= md.Venture_Cards[ 45] != 0?":o:":"   " #> | Misadventure! You drop your wallet and lose 10% of your ready cash!                                              |
|  47 | <#= md.Venture_Cards[ 46] != 0?":o:":"   " #> | Dicey adventure! Roll 2-6 to get all the suits. Roll 1 and lose all your suits.                                  |
|  48 | <#= md.Venture_Cards[ 47] != 0?":o:":"   " #> | Misadventure! All shops in a district picked at random fall in value by 10%!                                     |
|  49 | <#= md.Venture_Cards[ 48] != 0?":o:":"   " #> | Misadventure! All shops in a district picked at random fall in value by 20%!                                     |
|  50 | <#= md.Venture_Cards[ 49] != 0?":o:":"   " #> | Venture on! Move forward the same number of squares again.                                                       |
|  51 | <#= md.Venture_Cards[ 50] != 0?":o:":"   " #> | Venture on! Move forward 1 square more.                                                                          |
|  52 | <#= md.Venture_Cards[ 51] != 0?":o:":"   " #> | Venture on! Move forward another 2 squares.                                                                      |
|  53 | <#= md.Venture_Cards[ 52] != 0?":o:":"   " #> | Venture through space! Zoom over to the bank!                                                                    |
|  54 | <#= md.Venture_Cards[ 53] != 0?":o:":"   " #> | Venture through space! Pay 100G to zoom straight to the bank!                                                    |
|  55 | <#= md.Venture_Cards[ 54] != 0?":o:":"   " #> | Venture on! Roll the die again and move forward (with an invitation to browse thrown in!).                       |
|  56 | <#= md.Venture_Cards[ 55] != 0?":o:":"   " #> | Venture on! Roll the die again and move forward (with a half-price special offer thrown in!).                    |
|  57 | <#= md.Venture_Cards[ 56] != 0?":o:":"   " #> | Venture through space! Zoom to any square you like.                                                              |
|  58 | <#= md.Venture_Cards[ 57] != 0?":o:":"   " #> | Venture through space! Pay 100G to zoom to any non-venture, non-suit square you like!                            |
|  59 | <#= md.Venture_Cards[ 58] != 0?":o:":"   " #> | Stock venture! You can buy stocks in a district of your choice at 10% below the market value.                    |
|  60 | <#= md.Venture_Cards[ 59] != 0?":o:":"   " #> | Random venture! Stock prices increase by 10% in a district picked at random!                                     |
|  61 | <#= md.Venture_Cards[ 60] != 0?":o:":"   " #> | Special bonus! You receive a 10% dividend on your stocks!                                                        |
|  62 | <#= md.Venture_Cards[ 61] != 0?":o:":"   " #> | Special bonus! You receive a 20% dividend on your stocks!                                                        |
|  63 | <#= md.Venture_Cards[ 62] != 0?":o:":"   " #> | Random venture! Stock prices increase by 20% in a district picked at random!                                     |
|  64 | <#= md.Venture_Cards[ 63] != 0?":o:":"   " #> | Random venture! Stock prices increase by 30% in a district picked at random!                                     |
|  65 | <#= md.Venture_Cards[ 64] != 0?":o:":"   " #> | Forced buyout! You can buy a vacant plot or shop for five times its value, whether someone else owns it or not.  |
|  66 | <#= md.Venture_Cards[ 65] != 0?":o:":"   " #> | Special bonus! You receive 10 of the most valuable stocks!                                                       |
|  67 | <#= md.Venture_Cards[ 66] != 0?":o:":"   " #> | Stock venture! You can buy stocks in a district of your choice.                                                  |
|  68 | <#= md.Venture_Cards[ 67] != 0?":o:":"   " #> | Special arcade adventure! You're invited to play Memory Block!                                                   |
|  69 | <#= md.Venture_Cards[ 68] != 0?":o:":"   " #> | Stock venture! You can sell stocks you own at 20% above the market value.                                        |
|  70 | <#= md.Venture_Cards[ 69] != 0?":o:":"   " #> | Special bonus! You get a sudden promotion and receive a salary! (You lose any suits you have.)                   |
|  71 | <#= md.Venture_Cards[ 70] != 0?":o:":"   " #> | Capital venture! You can invest up to 200G of the bank's money in your shops.                                    |
|  72 | <#= md.Venture_Cards[ 71] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 to take 20 times the number of your shops in gold coins. Roll 2/4/6 to pay the same. |
|  73 | <#= md.Venture_Cards[ 72] != 0?":o:":"   " #> | Property venture! You can buy any unowned shop or vacant plot.                                                   |
|  74 | <#= md.Venture_Cards[ 73] != 0?":o:":"   " #> | Misadventure! You are forced to auction one of your shops (with a starting price of twice the shop's value).     |
|  75 | <#= md.Venture_Cards[ 74] != 0?":o:":"   " #> | Property venture! You can buy any unowned shop or vacant plot for twice its value.                               |
|  76 | <#= md.Venture_Cards[ 75] != 0?":o:":"   " #> | Special arcade adventure! You're invited to play Round the Blocks!                                               |
|  77 | <#= md.Venture_Cards[ 76] != 0?":o:":"   " #> | Freebie! Take five of each district's stocks.                                                                    |
|  78 | <#= md.Venture_Cards[ 77] != 0?":o:":"   " #> | Property venture! You can buy any unowned shop or vacant plot for 200G more than its value.                      |
|  79 | <#= md.Venture_Cards[ 78] != 0?":o:":"   " #> | Forced buyout! You can buy a vacant plot or shop for three times its value, whether someone else owns it or not. |
|  80 | <#= md.Venture_Cards[ 79] != 0?":o:":"   " #> | Freebie! Take a Spade!                                                                                           |
|  81 | <#= md.Venture_Cards[ 80] != 0?":o:":"   " #> | Misadventure! All other players can only move forward 1 on their next turn.                                      |
|  82 | <#= md.Venture_Cards[ 81] != 0?":o:":"   " #> | Freebie! Take a Club!                                                                                            |
|  83 | <#= md.Venture_Cards[ 82] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 and warp to a random location. Roll 2/4/6 and everyone else warps.                   |
|  84 | <#= md.Venture_Cards[ 83] != 0?":o:":"   " #> | Moneymaking venture! The winning player must pay you 10% of their ready cash!                                    |
|  85 | <#= md.Venture_Cards[ 84] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 85 times the number shown in gold coins from the bank!                 |
|  86 | <#= md.Venture_Cards[ 85] != 0?":o:":"   " #> | Moneymaking venture! Take 100G from all other players!                                                           |
|  87 | <#= md.Venture_Cards[ 86] != 0?":o:":"   " #> | Venture on! Roll the special all-7s-and-8s die and move forward again.                                           |
|  88 | <#= md.Venture_Cards[ 87] != 0?":o:":"   " #> | Misadventure! All other players swap places!                                                                     |
|  89 | <#= md.Venture_Cards[ 88] != 0?":o:":"   " #> | Freebie! All players take a Suit Yourself card!                                                                  |
|  90 | <#= md.Venture_Cards[ 89] != 0?":o:":"   " #> | Price hike venture! All shop prices go up by 30% until your next turn.                                           |
|  91 | <#= md.Venture_Cards[ 90] != 0?":o:":"   " #> | Cameo adventure! A healslime appears!                                                                            |
|  92 | <#= md.Venture_Cards[ 91] != 0?":o:":"   " #> | Cameo adventure! Lakitu appears!                                                                                 |
|  93 | <#= md.Venture_Cards[ 92] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 and your shops expand by 10%. Roll 2/4/6 and everyone else's shops expand by 5%.     |
|  94 | <#= md.Venture_Cards[ 93] != 0?":o:":"   " #> | Freebie! Take a Diamond!                                                                                         |
|  95 | <#= md.Venture_Cards[ 94] != 0?":o:":"   " #> | Misadventure! You throw an impromptu party. All other players come to your location!                             |
|  96 | <#= md.Venture_Cards[ 95] != 0?":o:":"   " #> | Misadventure! All players scramble to another player's location!                                                 |
|  97 | <#= md.Venture_Cards[ 96] != 0?":o:":"   " #> | Stock rise venture! Increase stock value by 20% in a district of your choice.                                    |
|  98 | <#= md.Venture_Cards[ 97] != 0?":o:":"   " #> | Forced buyout! You can buy a vacant plot or shop for four times its value, whether someone else owns it or not.  |
|  99 | <#= md.Venture_Cards[ 98] != 0?":o:":"   " #> | Freebie! What's inside...?                                                                                       |
| 100 | <#= md.Venture_Cards[ 99] != 0?":o:":"   " #> | Freebie! Take a Suit Yourself card!                                                                              |
| 101 | <#= md.Venture_Cards[100] != 0?":o:":"   " #> | Special bonus! Your shops all grow by 21%!                                                                       |
| 102 | <#= md.Venture_Cards[101] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 33 times the number shown in gold coins from all other players!        |
| 103 | <#= md.Venture_Cards[102] != 0?":o:":"   " #> | Misadventure! The values of all your shops drop by 25%!                                                          |
| 104 | <#= md.Venture_Cards[103] != 0?":o:":"   " #> | Misadventure! You give everyone 80G each!                                                                        |
| 105 | <#= md.Venture_Cards[104] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get the number shown x your level x 40G from the bank!                     |
| 106 | <#= md.Venture_Cards[105] != 0?":o:":"   " #> | Freebie! Roll the die and get half the number shown of Suit Yourself cards! (Decimals will be rounded down.)     |
| 107 | <#= md.Venture_Cards[106] != 0?":o:":"   " #> | Revaluation venture! You can expand any one of your shops by 30%.                                                |
| 108 | <#= md.Venture_Cards[107] != 0?":o:":"   " #> | Cashback venture! You can sell a shop back to the bank for four times its shop value.                            |
| 109 | <#= md.Venture_Cards[108] != 0?":o:":"   " #> | Revaluation venture! You can expand any one of your shops by 75%.                                                |
| 110 | <#= md.Venture_Cards[109] != 0?":o:":"   " #> | Special bonus! You receive 77 times the number of shops you own in gold coins from the bank!                     |
| 111 | <#= md.Venture_Cards[110] != 0?":o:":"   " #> | Cashback venture! You can sell a shop back to the bank for 500G more than its shop value.                        |
| 112 | <#= md.Venture_Cards[111] != 0?":o:":"   " #> | Special bonus! You receive 100 times the number of shops you own in gold coins!                                  |
| 113 | <#= md.Venture_Cards[112] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get the number shown x your level x 20G from the bank!                     |
| 114 | <#= md.Venture_Cards[113] != 0?":o:":"   " #> | Moneymaking venture! Take your level times 40G from all other players!                                           |
| 115 | <#= md.Venture_Cards[114] != 0?":o:":"   " #> | Misadventure! All other players can only move forward 7 on their next turn.                                      |
| 116 | <#= md.Venture_Cards[115] != 0?":o:":"   " #> | Moneymaking venture! Roll the die and get 60 times the number shown in gold coins from the player in 1st place!  |
| 117 | <#= md.Venture_Cards[116] != 0?":o:":"   " #> | Adventurous turning point! Everyone gets to choose which way to move on their next go.                           |
| 118 | <#= md.Venture_Cards[117] != 0?":o:":"   " #> | Lucky venture! You get a really big commission until your next turn!                                             |
| 119 | <#= md.Venture_Cards[118] != 0?":o:":"   " #> | Misadventure! You give 20% of your ready cash to the player in last place!                                       |
| 120 | <#= md.Venture_Cards[119] != 0?":o:":"   " #> | Misadventure! You drop your wallet and lose 20% of your ready cash!                                              |
| 121 | <#= md.Venture_Cards[120] != 0?":o:":"   " #> | Capital venture! You can invest up to 400G of the bank's money in your shops.                                    |
| 122 | <#= md.Venture_Cards[121] != 0?":o:":"   " #> | Moneymaking venture! The winning player must pay you 20% of their ready cash!                                    |
| 123 | <#= md.Venture_Cards[122] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 and your shops expand by 20%. Roll 2/4/6 and everyone else's shops expand by 5%.     |
| 124 | <#= md.Venture_Cards[123] != 0?":o:":"   " #> | Suit venture! Buy a Suit Yourself card for 50G.                                                                  |
| 125 | <#= md.Venture_Cards[124] != 0?":o:":"   " #> | Dicey adventure! Roll 1/3/5 to warp to a boon square. Roll 2/4/6 to warp to the arcade.                          |
| 126 | <#= md.Venture_Cards[125] != 0?":o:":"   " #> | Revaluation venture! Roll the die and expand your shops by 2% for each number.                                   |
| 127 | <#= md.Venture_Cards[126] != 0?":o:":"   " #> | Special arcade adventure! You're invited to play Round the Blocks and Memory Block!                              |
| 128 | <#= md.Venture_Cards[127] != 0?":o:":"   " #> | Special bonus! You receive 55 times the number of shops you own in gold coins from the bank!                     |

</details>

## Prerequisites

- <Placeholder for prerequisites>

## Changelog

### v1
- <Placeholder for version information>

## Authors

- <Placeholder for author information>