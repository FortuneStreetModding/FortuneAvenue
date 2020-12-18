using FSEditor.FSData;
using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class VanillaDatabase
    {
        public static byte[] getDefaultVentureCardTable(RuleSet ruleSet, ISet<SquareType> usedSquares)
        {
            string frequencyColumn = "";
            if (ruleSet == RuleSet.Standard)
                frequencyColumn = "Standard Mode Frequency";
            else if (ruleSet == RuleSet.Easy)
                frequencyColumn = "Easy Mode Frequency";
            var relevantVentureCards = new Dictionary<byte, uint>();
            foreach (var row in getVentureCardTable().Rows)
            {
                var dataRow = (DataRow)row;
                var id = dataRow.Field<byte>("Id");
                var frequency = dataRow.Field<uint>(frequencyColumn);
                var neededSquareTypes = dataRow.Field<SquareType[]>("Needed Square Types");
                if(neededSquareTypes == null || usedSquares.IsSupersetOf(neededSquareTypes))
                    relevantVentureCards.Add(id, frequency);
            }
            var sortedVentureCards = (from entry in relevantVentureCards orderby entry.Value descending select entry.Key).Take(64);
            byte[] ventureCards = new byte[128];
            foreach(var i in sortedVentureCards)
            {
                ventureCards[i-1] = 1;
            }
            return ventureCards;
        }
        public static int hasProblemWithVentureCardMissingNeededSquareType(byte[] ventureCards, ISet<SquareType> usedSquares)
        {
            foreach (var row in getVentureCardTable().Rows)
            {
                var dataRow = (DataRow)row;
                var id = dataRow.Field<byte>("Id");
                if(ventureCards[id - 1] == 1)
                {
                    var neededSquareTypes = dataRow.Field<SquareType[]>("Needed Square Types");
                    if (neededSquareTypes != null && !usedSquares.IsSupersetOf(neededSquareTypes))
                    {
                        return (int)id;
                    }
                }
            }
            return -1;
        }
        public static DataTable getVentureCardTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(byte));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Standard Mode Frequency", typeof(UInt32)); // In how many different vanilla standard maps that particular venture card has been enabled
            table.Columns.Add("Easy Mode Frequency", typeof(UInt32)); // In how many different vanilla easy maps that particular venture card has been enabled
            table.Columns.Add("Needed Square Types", typeof(SquareType[]));
            table.Rows.Add(   1, "Adventurous turning point! You can choose which way to move on your next go, (player's name)."                    , 13 , 15 );
            table.Rows.Add(   2, "Venture on! Roll the die again and move forward."                                                                 , 16 , 17 );
            table.Rows.Add(   3, "Venture through space! Zoom over to any non-venture, non-suit square you like!"                                   , 16 , 17 );
            table.Rows.Add(   4, "Moneymaking venture! Roll the die and get 40 times the number shown in gold coins from the player in 1st place!"  ,  8 , 11 );
            table.Rows.Add(   5, "Venture through space! Zoom over to any shop or vacant plot!"                                                     , 16 , 17 );
            table.Rows.Add(   6, "Venture through space! Zoom over to any venture or suit square!"                                                  , 15 , 16 );
            table.Rows.Add(   7, "Special bonus! Your shops all grow by 7%!"                                                                        , 14 , 13 );
            table.Rows.Add(   8, "Venture on! Everyone's shop prices increase by 30%! Now roll the die and move again."                             , 17 , 16 );
            table.Rows.Add(   9, "Venture on! Everyone's shops close for the day! Now roll the die and move again."                                 , 13 , 15 );
            table.Rows.Add(  10, "Venture on! Everyone's shop prices cut in half! Now roll the die and move again."                                 , 14 , 15 );
            table.Rows.Add(  11, "Moneymaking venture! Roll the die and get 11 times the number shown in gold coins from all other players!"        , 16 , 14 );
            table.Rows.Add(  12, "Capital venture! You can invest capital in any of your shops."                                                    , 13 , 11 );
            table.Rows.Add(  13, "Misadventure! The values of all your shops drop by 13%!"                                                          , 14 ,  9 );
            table.Rows.Add(  14, "Misadventure! You give everyone 30G each!"                                                                        ,  9 , 13 );
            table.Rows.Add(  15, "Moneymaking venture! Roll the die and get 50 times the number shown in gold coins from the bank!"                 , 17 , 16 );
            table.Rows.Add(  16, "Random venture! Shops expand in three districts picked at random!"                                                ,  7 ,  0 );
            table.Rows.Add(  17, "Special bonus! You receive half of your salary!"                                                                  , 14 , 14 );
            table.Rows.Add(  18, "Misadventure! The bank is forcibly buying you out! You're compelled to sell a shop for only twice its value."     , 13 , 13 );
            table.Rows.Add(  19, "Price hike venture! Your shop prices go up by 30% until your next turn."                                          , 15 , 11 );
            table.Rows.Add(  20, "Revaluation venture! You can expand any one of your shops by 20%."                                                , 14 , 15 );
            table.Rows.Add(  21, "Random venture! You receive 20 stocks in a district picked at random!"                                            , 12 ,  0 );
            table.Rows.Add(  22, "Cashback venture! You can sell a shop back to the bank for twice its shop value."                                 , 15 , 15 );
            table.Rows.Add(  23, "Revaluation venture! You can expand any one of your shops by 50%."                                                , 14 , 12 );
            table.Rows.Add(  24, "Misadventure! The bank is forcibly buying you out! You're compelled to sell a shop for 200G more than its value." ,  8 , 14 );
            table.Rows.Add(  25, "Misadventure! Your shop prices halve until your next turn!"                                                       , 15 , 13 );
            table.Rows.Add(  26, "Lucky venture! You get a big commission until your next turn!"                                                    , 13 , 15 );
            table.Rows.Add(  27, "Special bonus! You receive 27 times the number of shops you own in gold coins from the bank!"                     , 18 , 14 );
            table.Rows.Add(  28, "Cameo adventure! A goodybag appears!"                                                                             ,  7 ,  7 );
            table.Rows.Add(  29, "Freebie! Take a Heart!"                                                                                           , 15 , 15 );
            table.Rows.Add(  30, "Venture on! All shops charge a 100G flat rate! Now roll the die and move again."                                  , 15 , 12 );
            table.Rows.Add(  31, "Random venture! Shops expand by 10% in a district picked at random!"                                              , 13 ,  0 );
            table.Rows.Add(  32, "Random venture! Shops expand by 20% in a district picked at random!"                                              , 10 ,  0 );
            table.Rows.Add(  33, "Cashback venture! You can sell a shop back to the bank for three times its shop value."                           , 16 , 17 );
            table.Rows.Add(  34, "Dicey adventure! Roll 1/3/5 and your shops close for the day. Roll 2/4/6 and everyone else's shops close."        , 11 ,  9 );
            table.Rows.Add(  35, "Stock venture! You can sell stocks you own at 35% above the market value."                                        ,  8 ,  0 );
            table.Rows.Add(  36, "Capital venture! You can pay 100G for the chance to invest in your shops."                                        , 13 ,  3 );
            table.Rows.Add(  37, "Random venture! Shops expand by 30% in a district picked at random!"                                              ,  7 ,  0 );
            table.Rows.Add(  38, "Stock venture! You can buy stocks in a district of your choice at 10% above the market value."                    , 10 ,  0 );
            table.Rows.Add(  39, "Suit venture! Buy a Suit Yourself card for 100G."                                                                 , 12 , 16 );
            table.Rows.Add(  40, "Misadventure! You give away 10% of your ready cash to the player in last place!"                                  , 11 , 16 );
            table.Rows.Add(  41, "Misadventure! Stock prices fall by 10% in a district picked at random!"                                           , 13 ,  0 );
            table.Rows.Add(  42, "Misadventure! Stock prices fall by 20% in a district picked at random!"                                           , 12 ,  0 );
            table.Rows.Add(  43, "Misadventure! You pay an assets tax of two gold coins per unit of stock that you own!"                            , 18 ,  0 );
            table.Rows.Add(  44, "Misadventure! Roll the die and pay 44 times the number in gold coins to the player in last place!"                ,  7 , 16 );
            table.Rows.Add(  45, "Dicey adventure! Roll 1/3/5 to warp to a take-a-break square. Roll 2/4/6 to warp to the arcade."                  , 14 ,  3 , new SquareType[] { SquareType.TakeABreakSquare, SquareType.ArcadeSquare} );
            table.Rows.Add(  46, "Misadventure! You drop your wallet and lose 10% of your ready cash!"                                              , 11 ,  9 );
            table.Rows.Add(  47, "Dicey adventure! Roll 2-6 to get all the suits. Roll 1 and lose all your suits."                                  ,  4 ,  5 );
            table.Rows.Add(  48, "Misadventure! All shops in a district picked at random fall in value by 10%!"                                     , 12 ,  0 );
            table.Rows.Add(  49, "Misadventure! All shops in a district picked at random fall in value by 20%!"                                     ,  9 ,  0 );
            table.Rows.Add(  50, "Venture on! Move forward the same number of squares again."                                                       , 14 , 16 );
            table.Rows.Add(  51, "Venture on! Move forward 1 square more."                                                                          ,  7 , 10 );
            table.Rows.Add(  52, "Venture on! Move forward another 2 squares."                                                                      ,  7 ,  8 );
            table.Rows.Add(  53, "Venture through space! Zoom over to the bank!"                                                                    , 10 ,  3 );
            table.Rows.Add(  54, "Venture through space! Pay 100G to zoom straight to the bank!"                                                    , 11 , 10 );
            table.Rows.Add(  55, "Venture on! Roll the die again and move forward (with an invitation to browse thrown in!)."                       , 17 , 16 );
            table.Rows.Add(  56, "Venture on! Roll the die again and move forward (with a half-price special offer thrown in!)."                    , 17 , 17 );
            table.Rows.Add(  57, "Venture through space! Zoom to any square you like."                                                              , 16 , 16 );
            table.Rows.Add(  58, "Venture through space! Pay 100G to zoom to any non-venture, non-suit square you like!"                            ,  7 , 13 );
            table.Rows.Add(  59, "Stock venture! You can buy stocks in a district of your choice at 10% below the market value."                    , 14 ,  0 );
            table.Rows.Add(  60, "Random venture! Stock prices increase by 10% in a district picked at random!"                                     , 13 ,  0 );
            table.Rows.Add(  61, "Special bonus! You receive a 10% dividend on your stocks!"                                                        , 14 ,  0 );
            table.Rows.Add(  62, "Special bonus! You receive a 20% dividend on your stocks!"                                                        ,  7 ,  0 );
            table.Rows.Add(  63, "Random venture! Stock prices increase by 20% in a district picked at random!"                                     , 15 ,  0 );
            table.Rows.Add(  64, "Random venture! Stock prices increase by 30% in a district picked at random!"                                     ,  4 ,  0 );
            table.Rows.Add(  65, "Forced buyout! You can buy a vacant plot or shop for five times its value, whether someone else owns it or not."  ,  8 , 18 );
            table.Rows.Add(  66, "Special bonus! You receive 10 of the most valuable stocks!"                                                       ,  7 ,  0 );
            table.Rows.Add(  67, "Stock venture! You can buy stocks in a district of your choice."                                                  , 12 ,  0 );
            table.Rows.Add(  68, "Special arcade adventure! You're invited to play Memory Block!"                                                   ,  6 ,  1 );
            table.Rows.Add(  69, "Stock venture! You can sell stocks you own at 20% above the market value."                                        ,  6 ,  0 );
            table.Rows.Add(  70, "Special bonus! You get a sudden promotion and receive a salary! (You lose any suits you have.)"                   , 12 ,  4 );
            table.Rows.Add(  71, "Capital venture! You can invest up to 200G of the bank's money in your shops."                                    , 13 , 13 );
            table.Rows.Add(  72, "Dicey adventure! Roll 1/3/5 to take 20 times the number of your shops in gold coins. Roll 2/4/6 to pay the same." , 15 , 15 );
            table.Rows.Add(  73, "Property venture! You can buy any unowned shop or vacant plot."                                                   , 15 , 13 );
            table.Rows.Add(  74, "Misadventure! You are forced to auction one of your shops (with a starting price of twice the shop's value)."     ,  9 , 11 );
            table.Rows.Add(  75, "Property venture! You can buy any unowned shop or vacant plot for twice its value."                               , 11 , 11 );
            table.Rows.Add(  76, "Special arcade adventure! You're invited to play Round the Blocks!"                                               ,  7 , 12 );
            table.Rows.Add(  77, "Freebie! Take five of each district's stocks."                                                                    , 12 ,  0 );
            table.Rows.Add(  78, "Property venture! You can buy any unowned shop or vacant plot for 200G more than its value."                      , 12 , 12 );
            table.Rows.Add(  79, "Forced buyout! You can buy a vacant plot or shop for three times its value, whether someone else owns it or not." ,  8 , 18 );
            table.Rows.Add(  80, "Freebie! Take a Spade!"                                                                                           , 15 , 14 );
            table.Rows.Add(  81, "Misadventure! All other players can only move forward 1 on their next turn."                                      , 12 , 11 );
            table.Rows.Add(  82, "Freebie! Take a Club!"                                                                                            , 16 , 11 );
            table.Rows.Add(  83, "Dicey adventure! Roll 1/3/5 and warp to a random location. Roll 2/4/6 and everyone else warps."                   ,  9 ,  8 );
            table.Rows.Add(  84, "Moneymaking venture! The winning player must pay you 10% of their ready cash!"                                    , 12 , 17 );
            table.Rows.Add(  85, "Moneymaking venture! Roll the die and get 85 times the number shown in gold coins from the bank!"                 ,  6 ,  7 );
            table.Rows.Add(  86, "Moneymaking venture! Take 100G from all other players!"                                                           , 12 , 16 );
            table.Rows.Add(  87, "Venture on! Roll the special all-7s-and-8s die and move forward again."                                           ,  7 , 13 );
            table.Rows.Add(  88, "Misadventure! All other players swap places!"                                                                     ,  6 ,  6 );
            table.Rows.Add(  89, "Freebie! All players take a Suit Yourself card!"                                                                  ,  9 ,  9 );
            table.Rows.Add(  90, "Price hike venture! All shop prices go up by 30% until your next turn."                                           ,  5 ,  5 );
            table.Rows.Add(  91, "Cameo adventure! A healslime appears!"                                                                            ,  8 ,  6 );
            table.Rows.Add(  92, "Cameo adventure! Lakitu appears!"                                                                                 ,  3 ,  3 );
            table.Rows.Add(  93, "Dicey adventure! Roll 1/3/5 and your shops expand by 10%. Roll 2/4/6 and everyone else's shops expand by 5%."     ,  2 ,  7 );
            table.Rows.Add(  94, "Freebie! Take a Diamond!"                                                                                         , 15 , 13 );
            table.Rows.Add(  95, "Misadventure! You throw an impromptu party. All other players come to your location!"                             ,  7 ,  9 );
            table.Rows.Add(  96, "Misadventure! All players scramble to another player's location!"                                                 ,  6 ,  7 );
            table.Rows.Add(  97, "Stock rise venture! Increase stock value by 20% in a district of your choice."                                    ,  6 ,  0 );
            table.Rows.Add(  98, "Forced buyout! You can buy a vacant plot or shop for four times its value, whether someone else owns it or not."  , 10 , 18 );
            table.Rows.Add(  99, "Freebie! What's inside...?"                                                                                       , 10 ,  7 );
            table.Rows.Add( 100, "Freebie! Take a Suit Yourself card!"                                                                              , 17 , 15 );
            table.Rows.Add( 101, "Special bonus! Your shops all grow by 21%!"                                                                       ,  0 ,  4 );
            table.Rows.Add( 102, "Moneymaking venture! Roll the die and get 33 times the number shown in gold coins from all other players!"        ,  0 , 12 );
            table.Rows.Add( 103, "Misadventure! The values of all your shops drop by 25%!"                                                          ,  0 ,  3 );
            table.Rows.Add( 104, "Misadventure! You give everyone 80G each!"                                                                        ,  0 ,  9 );
            table.Rows.Add( 105, "Moneymaking venture! Roll the die and get the number shown x your level x 40G from the bank!"                     ,  0 , 15 );
            table.Rows.Add( 106, "Freebie! Roll the die and get half the number shown of Suit Yourself cards! (Decimals will be rounded down.)"     ,  2 , 13 );
            table.Rows.Add( 107, "Revaluation venture! You can expand any one of your shops by 30%."                                                ,  0 , 12 );
            table.Rows.Add( 108, "Cashback venture! You can sell a shop back to the bank for four times its shop value."                            ,  2 , 12 );
            table.Rows.Add( 109, "Revaluation venture! You can expand any one of your shops by 75%."                                                ,  0 ,  5 );
            table.Rows.Add( 110, "Special bonus! You receive 77 times the number of shops you own in gold coins from the bank!"                     ,  0 ,  7 );
            table.Rows.Add( 111, "Cashback venture! You can sell a shop back to the bank for 500G more than its shop value."                        ,  1 ,  9 );
            table.Rows.Add( 112, "Special bonus! You receive 100 times the number of shops you own in gold coins!"                                  ,  0 ,  5 );
            table.Rows.Add( 113, "Moneymaking venture! Roll the die and get the number shown x your level x 20G from the bank!"                     ,  1 , 14 );
            table.Rows.Add( 114, "Moneymaking venture! Take your level times 40G from all other players!"                                           ,  0 , 13 );
            table.Rows.Add( 115, "Misadventure! All other players can only move forward 7 on their next turn."                                      ,  1 , 12 );
            table.Rows.Add( 116, "Moneymaking venture! Roll the die and get 60 times the number shown in gold coins from the player in 1st place!"  ,  0 , 12 );
            table.Rows.Add( 117, "Adventurous turning point! Everyone gets to choose which way to move on their next go."                           ,  6 , 13 );
            table.Rows.Add( 118, "Lucky venture! You get a really big commission until your next turn!"                                             ,  0 , 10 );
            table.Rows.Add( 119, "Misadventure! You give 20% of your ready cash to the player in last place!"                                       ,  0 , 10 );
            table.Rows.Add( 120, "Misadventure! You drop your wallet and lose 20% of your ready cash!"                                              ,  0 ,  3 );
            table.Rows.Add( 121, "Capital venture! You can invest up to 400G of the bank's money in your shops."                                    ,  0 ,  5 );
            table.Rows.Add( 122, "Moneymaking venture! The winning player must pay you 20% of their ready cash!"                                    ,  0 ,  6 );
            table.Rows.Add( 123, "Dicey adventure! Roll 1/3/5 and your shops expand by 20%. Roll 2/4/6 and everyone else's shops expand by 5%."     ,  0 ,  4 );
            table.Rows.Add( 124, "Suit venture! Buy a Suit Yourself card for 50G."                                                                  ,  3 ,  8 );
            table.Rows.Add( 125, "Dicey adventure! Roll 1/3/5 to warp to a boon square. Roll 2/4/6 to warp to the arcade."                          ,  0 ,  4 , new SquareType[] { SquareType.BoonSquare, SquareType.ArcadeSquare });
            table.Rows.Add( 126, "Revaluation venture! Roll the die and expand your shops by 2% for each number."                                   ,  0 ,  4 );
            table.Rows.Add( 127, "Special arcade adventure! You're invited to play Round the Blocks and Memory Block!"                              ,  0 ,  4 );
            table.Rows.Add( 128, "Special bonus! You receive 55 times the number of shops you own in gold coins from the bank!"                     ,  0 ,  6 );
            return table;
        }
        public static DataTable getMapSetZoneOrdering()
        {
            DataTable table = new DataTable();
            table.Columns.Add("MapSet", typeof(int));
            table.Columns.Add("Zone", typeof(int));
            table.Columns.Add("Order", typeof(int));
            table.Columns.Add("Map Id", typeof(int));
            table.Rows.Add(0, 0, 0, 9);
            table.Rows.Add(0, 0, 1, 17);
            table.Rows.Add(0, 0, 2, 10);
            table.Rows.Add(0, 0, 3, 13);
            table.Rows.Add(0, 0, 4, 12);
            table.Rows.Add(0, 0, 5, 14);

            table.Rows.Add(0, 1, 0, 3);
            table.Rows.Add(0, 1, 1, 7);
            table.Rows.Add(0, 1, 2, 1);
            table.Rows.Add(0, 1, 3, 0);
            table.Rows.Add(0, 1, 4, 4);
            table.Rows.Add(0, 1, 5, 2);

            table.Rows.Add(0, 2, 0, 15);
            table.Rows.Add(0, 2, 1, 5);
            table.Rows.Add(0, 2, 2, 6);
            table.Rows.Add(0, 2, 3, 8);
            table.Rows.Add(0, 2, 4, 11);
            table.Rows.Add(0, 2, 5, 16);

            table.Rows.Add(1, 0, 0, 30);
            table.Rows.Add(1, 0, 1, 38);
            table.Rows.Add(1, 0, 2, 31);
            table.Rows.Add(1, 0, 3, 34);
            table.Rows.Add(1, 0, 4, 33);
            table.Rows.Add(1, 0, 5, 35);

            table.Rows.Add(1, 1, 0, 24);
            table.Rows.Add(1, 1, 1, 28);
            table.Rows.Add(1, 1, 2, 22);
            table.Rows.Add(1, 1, 3, 21);
            table.Rows.Add(1, 1, 4, 25);
            table.Rows.Add(1, 1, 5, 23);

            table.Rows.Add(1, 2, 0, 36);
            table.Rows.Add(1, 2, 1, 26);
            table.Rows.Add(1, 2, 2, 27);
            table.Rows.Add(1, 2, 3, 29);
            table.Rows.Add(1, 2, 4, 32);
            table.Rows.Add(1, 2, 5, 37);
            return table;
        }
        public static int getVanillaMapSet(int mapId)
        {
            try
            {
                var result = from row in VanillaDatabase.getMapSetZoneOrdering().AsEnumerable()
                             where row.Field<int>("Map Id") == mapId
                             select row.Field<int>("MapSet");
                if (result.Any())
                    return result.Distinct().Single();
            }
            catch (IndexOutOfRangeException e) { }
            return -1;
        }
        public static int getVanillaZone(int mapId)
        {
            try
            {
                var result = from row in VanillaDatabase.getMapSetZoneOrdering().AsEnumerable()
                             where row.Field<int>("Map Id") == mapId
                             select row.Field<int>("Zone");
                if (result.Any())
                    return result.Distinct().Single();
            }
            catch (IndexOutOfRangeException e) { }
            return -1;
        }
        public static int getVanillaOrder(int mapId)
        {
            try
            {
                var result = from row in VanillaDatabase.getMapSetZoneOrdering().AsEnumerable()
                             where row.Field<int>("Map Id") == mapId
                             select row.Field<int>("Order");
                if (result.Any())
                    return result.Distinct().Single();
            }
            catch (IndexOutOfRangeException e) { }
            return -1;
        }
        public static DataTable getMapTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Background", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Bgm Id", typeof(UInt32));
            table.Columns.Add("Map Icon", typeof(string));
            table.Columns.Add("Map Tpl", typeof(string));
            table.Rows.Add( "bg101"  , "Trodain Castle"    , 17 , "p_bg_101" , "ui_menu007_bg101.tpl" );
            table.Rows.Add( "bg109"  , "The Observatory"   , 21 , "p_bg_109" , "ui_menu007_bg109.tpl" );
            table.Rows.Add( "bg102"  , "Ghost Ship"        ,  3 , "p_bg_102" , "ui_menu007_bg102.tpl" );
            table.Rows.Add( "bg105"  , "Slimenia"          ,  6 , "p_bg_105" , "ui_menu007_bg105.tpl" );
            table.Rows.Add( "bg104"  , "Mt. Magmageddon"   ,  5 , "p_bg_104" , "ui_menu007_bg104.tpl" );
            table.Rows.Add( "bg106"  , "Robbin' Hood Ruins",  7 , "p_bg_106" , "ui_menu007_bg106.tpl" );
            table.Rows.Add( "bg004"  , "Mario Stadium"     , 12 , "p_bg_004" , "ui_menu007_bg004.tpl" );
            table.Rows.Add( "bg008"  , "Starship Mario"    , 15 , "p_bg_008" , "ui_menu007_bg008.tpl" );
            table.Rows.Add( "bg002"  , "Mario Circuit"     ,  0 , "p_bg_002" , "ui_menu007_bg002.tpl" );
            table.Rows.Add( "bg001"  , "Yoshi's Island"    , 11 , "p_bg_001" , "ui_menu007_bg001.tpl" );
            table.Rows.Add( "bg005"  , "Delfino Plaza"     , 13 , "p_bg_005" , "ui_menu007_bg005.tpl" );
            table.Rows.Add( "bg003"  , "Peach's Castle"    ,  1 , "p_bg_003" , "ui_menu007_bg003.tpl" );
            table.Rows.Add( "bg107"  , "Alefgard"          ,  9 , "p_bg_107" , "ui_menu007_bg107.tpl" );
            table.Rows.Add( "bg006"  , "Super Mario Bros"  , 14 , "p_bg_006" , "ui_menu007_bg006.tpl" );
            table.Rows.Add( "bg007"  , "Bowser's Castle"   ,  2 , "p_bg_007" , "ui_menu007_bg007.tpl" );
            table.Rows.Add( "bg009"  , "Good Egg Galaxy"   , 16 , "p_bg_009" , "ui_menu007_bg009.tpl" );
            table.Rows.Add( "bg103"  , "The Colossus"      ,  4 , "p_bg_103" , "ui_menu007_bg103.tpl" );
            table.Rows.Add( "bg103_e", "The Colossus Easy" ,  4 , "p_bg_103" , "ui_menu007_bg103.tpl" );
            table.Rows.Add( "bg108"  , "Alltrades Abbey"   , 19 , "p_bg_108" , "ui_menu007_bg108.tpl" );
            table.Rows.Add( "bg901"  , "Practice Board"    , 22 );
            return table;
        }
        public static DataTable getBgmTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Bgm Id", typeof(UInt32));
            table.Columns.Add("Brsar Mario", typeof(UInt32));
            table.Columns.Add("Brsar DragonQuest", typeof(UInt32));
            table.Columns.Add("Filename", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Rows.Add( 17 , 29 , 29 , "29_BGM_MAP_TRODAIN"       , "Trodain Castle"     );
            table.Rows.Add( 21 , 41 , 41 , "37_BGM_MAP_ANGEL"         , "The Observatory"    );
            table.Rows.Add(  3 , 31 , 31 , "30_BGM_MAP_GHOSTSHIP"     , "Ghost Ship"         );
            table.Rows.Add(  6 , 34 , 34 , "33_BGM_MAP_SLABACCA"      , "Slimenia"           );
            table.Rows.Add(  5 , 33 , 33 , "32_BGM_MAP_SINOKAZAN"     , "Mt. Magmageddon"    );
            table.Rows.Add(  7 , 35 , 35 , "34_BGM_MAP_KANDATA"       , "Robbin' Hood Ruins" );
            table.Rows.Add( 12 , 23 , 23 , "23_BGM_MAP_STADIUM"       , "Mario Stadium"      );
            table.Rows.Add( 15 , 27 , 27 , "27_BGM_MAP_STARSHIP"      , "Starship Mario"     );
            table.Rows.Add(  0 , 21 , 21 , "21_BGM_MAP_CIRCUIT"       , "Mario Circuit"      );
            table.Rows.Add( 11 , 20 , 20 , "20_BGM_MAP_YOSHI"         , "Yoshi's Island"     );
            table.Rows.Add( 13 , 24 , 24 , "24_BGM_MAP_DOLPIC"        , "Delfino Plaza"      );
            table.Rows.Add(  1 , 22 , 22 , "22_BGM_MAP_PEACH"         , "Peach's Castle"     );
            table.Rows.Add(  9 , 37 , 37 , "35_BGM_MAP_ALEFGARD"      , "Alefgard"           );
            table.Rows.Add( 14 , 25 , 25 , "25_BGM_MAP_SMB"           , "Super Mario Bros"   );
            table.Rows.Add(  2 , 26 , 26 , "26_BGM_MAP_KOOPA"         , "Bowser's Castle"    );
            table.Rows.Add( 16 , 28 , 28 , "28_BGM_MAP_EGG"           , "Good Egg Galaxy"    );
            table.Rows.Add(  4 , 32 , 32 , "31_BGM_MAP_MAJINZOU"      , "The Colossus"       );
            table.Rows.Add( 19 , 39 , 39 , "36_BGM_MAP_DHAMA"         , "Alltrades Abbey"    );
            table.Rows.Add( 22 ,  5 ,  5 , "05_BGM_MENU"              , "Practice Board"     );
            table.Rows.Add(  8 , 36 , 36 , "34_BGM_MAP_KANDATA_old"   , "Unused"             );
            table.Rows.Add( 10 , 38 , 38 , "35_BGM_MAP_ALEFGARD_old"  , "Unused"             );
            table.Rows.Add( 18 , 30 , 30 , "29_BGM_MAP_TRODAIN_old"   , "Unused"             );
            table.Rows.Add( 20 , 40 , 40 , "36_BGM_MAP_DHAMA_old"     , "Unused"             );
            table.Rows.Add( 23 , 42 , 43 , "38_BGM_GOALPROP_(M/D)"    , "Promotion"          );
            table.Rows.Add( 24 , 10 , 11 , "10_BGM_WINNER_(M/D)"      , "Winner"             );
            table.Rows.Add( 25 , 12 , 12 , "12_BGM_CHANCECARD"        , "Select Chancecard"  );
            table.Rows.Add( 26 , 13 , 13 , "13_BGM_STOCK"             , "Buy/Sell Stock"     );
            table.Rows.Add( 27 , 14 , 14 , "14_BGM_AUCTION"           , "Auction"            );
            table.Rows.Add( 28 , 15 , 16 , "15_BGM_CASINO_SLOT_(M/D)" , "Round The Blocks"   );
            table.Rows.Add( 29 , 17 , 17 , "15_BGM_CASINO_BLOCK"      , "Memory Block"       );
            table.Rows.Add( 30 , 12 , 12 , "12_BGM_CHANCECARD"        , "Dart of Gold"       );
            table.Rows.Add( 31 , 16 , 16 , "16_BGM_CASINO_SLOT_D"     , "Select your Slime"  );
            table.Rows.Add( 32 , 19 , 19 , "19_BGM_CASINO_RACE"       , "Racing Slimes"      );
            table.Rows.Add( 33 ,  0 ,  0 , "01_BGM_TITLE"             , "Title Screen"       );
            table.Rows.Add( 34 ,  5 ,  5 , "05_BGM_MENU"              , "Menu"               );
            table.Rows.Add( 35 ,  3 ,  3 , "04_BGM_SAVELOAD"          , "Save/Load Screen"   );
            table.Rows.Add( 36 ,  4 ,  4 , "04_BGM_SAVELOAD_old"      , "Unused"             );
            table.Rows.Add( 37 ,  6 ,  6 , "06_BGM_WIFI"              , "Wi-Fi"              );
            table.Rows.Add( 38 ,  3 ,  3 , "04_BGM_SAVELOAD"          , "Unknown"            );
            table.Rows.Add( 39 ,  7 ,  7 , "07_BGM_ENDING_M"          , "Credits"            );
            return table;
        }

        public static Optional<string> getMapIconFromVanillaBackground(string background)
        {
            try
            {
                var result = from row in VanillaDatabase.getMapTable().AsEnumerable()
                             where row.Field<string>("Background") == background
                             select row.Field<string>("Map Icon");
                if (result.Any())
                    return Optional<string>.Create(result.Distinct().Single());
            }
            catch (IndexOutOfRangeException e) { }
            return Optional<string>.CreateEmpty();
        }

        public static Optional<UInt32> getBgmIdFromVanillaBackground(string background)
        {
            try
            {
                var result = from row in VanillaDatabase.getMapTable().AsEnumerable()
                             where row.Field<string>("Background") == background
                             select row.Field<UInt32>("Bgm Id");
                if (result.Any())
                    return Optional<UInt32>.Create(result.Distinct().Single());
            }
            catch (IndexOutOfRangeException e) { }
            return Optional<UInt32>.CreateEmpty();
        }

        public static Optional<string> getVanillaTpl(string mapIcon)
        {
            try
            {
                var result = from row in VanillaDatabase.getMapTable().AsEnumerable()
                             where row.Field<string>("Map Icon") == mapIcon
                             select row.Field<string>("Map Tpl");
                if (result.Any())
                    return Optional<string>.Create(result.Distinct().Single());
            }
            catch (IndexOutOfRangeException e) { }
            return Optional<string>.CreateEmpty();
        }

        public static Dictionary<string, string> getVanillaMapIconToTplNameDictionary()
        {
            var dict = new Dictionary<string, string>();
            foreach (var row in getMapTable().Rows)
            {
                var dataRow = (DataRow)row;
                dict.Add(dataRow.Field<string>("Map Icon"), dataRow.Field<string>("Map Tpl"));
            }
            return dict;
        }
    }
}