using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Death.Items;
using HarmonyLib;
using MelonLoader;

namespace MQOD
{
    public static class Sort
    {
        private static readonly FieldInfo ItemGridSlotsAccessor = typeof(ItemGrid).GetField("_slots", AccessTools.all);

        public enum Category
        {
            UNIQUENESS,
            RARITY,
            TIER,
            TYPE
        }

        public class Ordering : List<Category>
        {
            
            public Func<Item, ulong> getRank;

            public Func<Item, ulong> generateRankingFunc()
            {
                return getRank;

                ulong getRank(Item item)
                {
                    ulong rank = 0b_0000000000000000000000000000000000000000000000000000000000000000;
                    if (item == null) return rank;

                    ulong mask = 0b_1000000000000000000000000000000000000000000000000000000000000000;
                    int bitsLeft = 64;

                    Enumerator enumerator = GetEnumerator();
                    for (int i = 0; i < Count; i++)
                    {
                        switch (enumerator.Current)
                        {
                            case Category.UNIQUENESS:
                                if (item.IsUnique) rank |= mask;
                                mask >>= 1;
                                bitsLeft -= 1;
                                break;
                            case Category.RARITY:
                                rank |= mask >> ((int)ItemRarity._Count - (int)item.Rarity);
                                mask >>= (int)ItemRarity._Count;
                                bitsLeft -= (int)ItemRarity._Count;
                                break;
                            case Category.TIER:
                                rank |= mask >> (TierId.Count - item.Tier.Id);
                                mask >>= TierId.Count;
                                bitsLeft -= TierId.Count;
                                break;
                            case Category.TYPE:
                                rank |= mask >> (int)item.Type;
                                mask >>= (int)ItemType._Count;
                                bitsLeft -= (int)ItemType._Count;

                                bitsLeft -= 26;
                                foreach (int c in item.SubtypeCode)
                                    switch (c)
                                    {
                                        case >= 65 when c <= 65 + bitsLeft:
                                            rank ^= mask >> (c - 65);
                                            break;
                                        case > 96 and < 123:
                                            rank ^= mask >> (c - 97 + bitsLeft);
                                            break;
                                    }

                                break;
                        }

                        if (!enumerator.MoveNext()) break;
                    }

                    enumerator.Dispose();

                    return rank;
                }
            }
        }

        public static bool sortItemGrid(ItemGrid itemGrid)
        {
            Dictionary<ulong, List<Item>> ItemRank = new();

            foreach (Item item in GetItemsWithNulls(itemGrid))
            {
                ulong rank = getRank(item);
                // ulong rank = generateRankingFunc()(item);
                if (!ItemRank.ContainsKey(rank)) ItemRank[rank] = new List<Item>();
                ItemRank[rank].Add(item);
            }

            ulong[] A = new List<ulong>(ItemRank.Keys).ToArray();
            if (A.Length < 2) return false;
            ulong[] A_copy = new ulong[A.Length]; // original 
            Array.Copy(A, A_copy, A.Length);
            SortArrayInPlace(A, 0, A.Length - 1);
            Array.Reverse(A);
            // Check if sorting is required
            if (!A.Where((t, j) => t != A_copy[j]).Any()) return false;

            itemGrid.Clear();
            int i = 0;
            foreach (ulong rank in A)
            foreach (Item item in ItemRank[rank])
            {
                int y = i / itemGrid.Width;
                int x = i % itemGrid.Width;
                itemGrid.Set(x, y, item);
                i++;
            }

            return true;
        }

        private static ulong getRank(Item item)
        {
            /* Uniqueness
             * Rarity
             * Tier
             * Type
             * SubType
             */


            // This method creates an absolute ordering (hopefully)  
            ulong rank = 0b_0000000000000000000000000000000000000000000000000000000000000000;
            if (item == null) return rank; // nulls at the end

            ulong mask = 0b_1000000000000000000000000000000000000000000000000000000000000000;
            int bitsLeft = 64;

            //  - 1 bit for uniqueness
            if (item.IsUnique) rank |= mask;
            mask >>= 1;
            bitsLeft -= 1;

            //  - 6 bits for rarity
            rank |= mask >> ((int)ItemRarity._Count - (int)item.Rarity);
            mask >>= (int)ItemRarity._Count;
            bitsLeft -= (int)ItemRarity._Count;

            /* Tier kind of beats rarity so this
             * should be reversed, first tier then rarity?
             * Pro: - better stats sorting
             * Con: - Looks ugly in inventory
             */

            //  - 5 bits for tiers 
            rank |= mask >> (TierId.Count - item.Tier.Id);
            mask >>= TierId.Count;
            bitsLeft -= TierId.Count;

            //  - 11 bits for type
            rank |= mask >> (int)item.Type;
            mask >>= (int)ItemType._Count;
            bitsLeft -= (int)ItemType._Count;

            //  - 41 bits for lowercase chars + [A,B,C,D,E,F,G,H,I,J,K,L,M,N,O] in subtype  
            // There's prob a smarter way 

            bitsLeft -= 26;
            foreach (int c in item.SubtypeCode)
                switch (c)
                {
                    case >= 65 when c <= 65 + bitsLeft:
                        rank ^= mask >> (c - 65);
                        break;
                    case > 96 and < 123:
                        rank ^= mask >> (c - 97 + bitsLeft);
                        break;
                }

            return rank;
        }

        // QuickSort 
        public static void SortArrayInPlace(ulong[] array, int leftIndex, int rightIndex)
        {
            int i = leftIndex;
            int j = rightIndex;
            ulong pivot = array[leftIndex];
            while (i <= j)
            {
                while (array[i] < pivot) i++;

                while (array[j] > pivot) j--;
                if (i <= j)
                {
                    ulong tmp = array[i]; 
                    array[i] = array[j];
                    array[j] = tmp; 
                    // (array[i], array[j]) = (array[j], array[i]);
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                SortArrayInPlace(array, leftIndex, j);
            if (i < rightIndex)
                SortArrayInPlace(array, i, rightIndex);
        }

        private static IEnumerable<Item> GetItemsWithNulls(ItemGrid itemGrid)
        {
            ItemSlot[] itemSlotArray = (ItemSlot[])ItemGridSlotsAccessor.GetValue(itemGrid);
            foreach (ItemSlot itemSlot in itemSlotArray)
            {
                if (itemSlot.IsFull)
                    yield return itemSlot.Item;
                else
                    yield return null;
            }
        }
    }
}