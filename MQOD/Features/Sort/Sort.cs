using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Death.Items;
using HarmonyLib;

namespace MQOD
{
    public static class Sort
    {
        public enum Category
        {
            UNIQUENESS,
            RARITY,
            TIER,
            TYPE,
            NULL
        }

        private static readonly FieldInfo ItemGridSlotsAccessor = typeof(ItemGrid).GetField("_slots", AccessTools.all);

        private static DynamicMethod getRankIL(Item item)
        {
            DynamicMethod dynamicMethod = new("getRank", typeof(long), new[] { typeof(Item) });

            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

            Label[] labels = new Label[11];
            // ulong num = 0uL;
            ilGenerator.Emit(OpCodes.Ldc_I4_0);
            ilGenerator.Emit(OpCodes.Conv_I8);
            ilGenerator.Emit(OpCodes.Stloc_0);
            // if (item == null)
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Brtrue_S, labels[0]);
            // return num;
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);
            // ulong num2 = 9223372036854775808uL;
            ilGenerator.MarkLabel(labels[0]);
            ilGenerator.Emit(OpCodes.Ldc_I8, -9223372036854775808);
            ilGenerator.Emit(OpCodes.Stloc_1);
            // int num3 = 64;
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 64);
            ilGenerator.Emit(OpCodes.Stloc_2);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            // if (item.IsUnique)
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(Item).GetProperty(nameof(Item.IsUnique))!.GetGetMethod(),
                new Type[] { });
            ilGenerator.Emit(OpCodes.Brfalse_S, labels[1]);
            // num |= num2;
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Or);
            ilGenerator.Emit(OpCodes.Stloc_0);
            // num2 >>= 1;
            ilGenerator.MarkLabel(labels[1]);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldc_I4_1);
            ilGenerator.Emit(OpCodes.Shr_Un);
            ilGenerator.Emit(OpCodes.Stloc_1);
            // num3--;
            ilGenerator.Emit(OpCodes.Ldloc_2);
            ilGenerator.Emit(OpCodes.Ldc_I4_1);
            ilGenerator.Emit(OpCodes.Sub);
            ilGenerator.Emit(OpCodes.Stloc_2);
            // num |= num2 >> (int)(6 - item.Rarity);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldc_I4_6);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(Item).GetProperty(nameof(Item.Rarity))!.GetGetMethod(),
                new Type[] { });
            ilGenerator.Emit(OpCodes.Sub);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 63);
            ilGenerator.Emit(OpCodes.And);
            ilGenerator.Emit(OpCodes.Shr_Un);
            ilGenerator.Emit(OpCodes.Or);
            ilGenerator.Emit(OpCodes.Stloc_0);
            // num2 >>= 6;
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldc_I4_6);
            ilGenerator.Emit(OpCodes.Shr_Un);
            ilGenerator.Emit(OpCodes.Stloc_1);
            // num3 -= 6;
            ilGenerator.Emit(OpCodes.Ldloc_2);
            ilGenerator.Emit(OpCodes.Ldc_I4_6);
            ilGenerator.Emit(OpCodes.Sub);
            ilGenerator.Emit(OpCodes.Stloc_2);
            // num |= num2 >> 5 - item.Tier.Id;
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldc_I4_5);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(Item).GetProperty(nameof(Item.Tier))!.GetGetMethod(),
                new Type[] { });
            ilGenerator.Emit(OpCodes.Stloc_3);
            ilGenerator.Emit(OpCodes.Ldloca_S, 3);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(TierId).GetProperty(nameof(TierId.Id))!.GetGetMethod(),
                new Type[] { });
            ilGenerator.Emit(OpCodes.Sub);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 63);
            ilGenerator.Emit(OpCodes.And);
            ilGenerator.Emit(OpCodes.Shr_Un);
            ilGenerator.Emit(OpCodes.Or);
            ilGenerator.Emit(OpCodes.Stloc_0);
            // num2 >>= 5;
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldc_I4_5);
            ilGenerator.Emit(OpCodes.Shr_Un);
            ilGenerator.Emit(OpCodes.Stloc_1);
            // num3 -= 5;
            ilGenerator.Emit(OpCodes.Ldloc_2);
            ilGenerator.Emit(OpCodes.Ldc_I4_5);
            ilGenerator.Emit(OpCodes.Sub);
            ilGenerator.Emit(OpCodes.Stloc_2);
            // num |= num2 >> (int)item.Type;
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(Item).GetProperty(nameof(Item.Type))!.GetGetMethod(),
                new Type[] { });
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 63);
            ilGenerator.Emit(OpCodes.And);
            ilGenerator.Emit(OpCodes.Shr_Un);
            ilGenerator.Emit(OpCodes.Or);
            // num2 >>= 11;
            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 11);
            ilGenerator.Emit(OpCodes.Shr_Un);
            ilGenerator.Emit(OpCodes.Stloc_1);
            // num3 -= 11;
            ilGenerator.Emit(OpCodes.Ldloc_2);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 11);
            ilGenerator.Emit(OpCodes.Sub);
            ilGenerator.Emit(OpCodes.Stloc_2);
            // num3 -= 26;
            ilGenerator.Emit(OpCodes.Ldloc_2);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 26);
            ilGenerator.Emit(OpCodes.Sub);
            ilGenerator.Emit(OpCodes.Stloc_2);
            // string subtypeCode = item.SubtypeCode;
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(Item).GetProperty(nameof(Item.SubtypeCode))!.GetGetMethod(),
                new Type[] { });
            ilGenerator.Emit(OpCodes.Stloc_S, 4);
            // (no C# code)
            ilGenerator.Emit(OpCodes.Ldc_I4_0);
            ilGenerator.Emit(OpCodes.Stloc_S, 5);
            // 	foreach (int num4 in subtypeCode)
            // 	{
            // 		int num5 = num4;
            // 		if (num5 < 65)
            // 		{
            // 			continue;
            // 		}
            // 		if (num4 > 65 + num3)
            // 		{
            // 			if (num5 > 96 && num5 < 123)
            // 			{
            // 				num ^= num2 >> num4 - 97 + num3;
            // 			}
            // 		}
            // 		else
            // 		{
            // 			num ^= num2 >> num4 - 65;
            // 		}
            // 	}
            ilGenerator.Emit(OpCodes.Br_S, labels[7]);
            ilGenerator.MarkLabel(labels[2]);
            ilGenerator.Emit(OpCodes.Ldloc_S, 4);
            ilGenerator.Emit(OpCodes.Ldloc_S, 5);
            ilGenerator.EmitCall(OpCodes.Callvirt,
                typeof(string).GetProperty("Item", new[] { typeof(int) })!.GetGetMethod(),
                new Type[] { });
            ilGenerator.Emit(OpCodes.Stloc_S, 6);
            // int num5 = num4;
            ilGenerator.Emit(OpCodes.Ldloc_S, 6);
            ilGenerator.Emit(OpCodes.Stloc_S, 7);
            // if (num5 < 65)
            ilGenerator.Emit(OpCodes.Ldloc_S, 7);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 65);
            ilGenerator.Emit(OpCodes.Bge_S, labels[4]);
            // if (num5 > 96 && num5 < 123)
            ilGenerator.Emit(OpCodes.Br_S, labels[6]);
            // loop start (head: IL_00b0)
            ilGenerator.MarkLabel(labels[3]);
            ilGenerator.Emit(OpCodes.Ldloc_S, 7);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 96);
            ilGenerator.Emit(OpCodes.Ble_S, labels[6]);
            ilGenerator.Emit(OpCodes.Ldloc_S, 7);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 123);
            ilGenerator.Emit(OpCodes.Blt_S, labels[5]);
            // if (num4 > 65 + num3)
            ilGenerator.Emit(OpCodes.Br_S, labels[6]);
            ilGenerator.MarkLabel(labels[4]);
            ilGenerator.Emit(OpCodes.Ldloc_S, 6);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 65);
            ilGenerator.Emit(OpCodes.Ldloc_2);
            ilGenerator.Emit(OpCodes.Add);
            ilGenerator.Emit(OpCodes.Bgt_S, labels[3]);
            // end loop
            // num ^= num2 >> num4 - 65;
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldloc_S, 6);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 65);
            ilGenerator.Emit(OpCodes.Sub);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 63);
            ilGenerator.Emit(OpCodes.And);
            ilGenerator.Emit(OpCodes.Shr_Un);
            ilGenerator.Emit(OpCodes.Xor);
            ilGenerator.Emit(OpCodes.Stloc_0);
            // num ^= num2 >> num4 - 97 + num3;
            ilGenerator.Emit(OpCodes.Br_S, labels[6]);
            ilGenerator.MarkLabel(labels[5]);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldloc_S, 6);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 97);
            ilGenerator.Emit(OpCodes.Sub);
            ilGenerator.Emit(OpCodes.Ldloc_2);
            ilGenerator.Emit(OpCodes.Add);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, 63);
            ilGenerator.Emit(OpCodes.And);
            ilGenerator.Emit(OpCodes.Shr_Un);
            ilGenerator.Emit(OpCodes.Xor);
            ilGenerator.Emit(OpCodes.Stloc_0);
            // (no C# code)
            ilGenerator.MarkLabel(labels[6]);
            ilGenerator.Emit(OpCodes.Ldloc_S, 5);
            ilGenerator.Emit(OpCodes.Ldc_I4_1);
            ilGenerator.Emit(OpCodes.Add);
            ilGenerator.Emit(OpCodes.Stloc_S, 5);
            // return num;
            ilGenerator.MarkLabel(labels[7]);
            ilGenerator.Emit(OpCodes.Ldloc_S, 5);
            ilGenerator.Emit(OpCodes.Ldloc_S, 4);
            ilGenerator.EmitCall(OpCodes.Callvirt,
                typeof(string).GetProperty(nameof(string.Length))!.GetGetMethod(),
                new Type[] { });
            ilGenerator.Emit(OpCodes.Blt_S, labels[2]);
            // end loop
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);

            return dynamicMethod;
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
                if (itemSlot.IsFull)
                    yield return itemSlot.Item;
                else
                    yield return null;
        }

        public class Ordering : List<Category>
        {
            public static readonly Ordering DEFAULT = _DEFAULT;

            public Ordering()
            {
            }

            public Ordering(int n)
            {
                for (int i = 0; i < n; i++) Add(Category.NULL);
            }

            private static Ordering _DEFAULT => new()
                { Category.UNIQUENESS, Category.RARITY, Category.TIER, Category.TYPE };

            public override string ToString()
            {
                return string.Join(">", this.Select(category => category.GetString()));
            }


            /* This can be done with compile time performance with DynamicMethod and OpCodes
             * TODO: Implement
             *
             */
            private ulong getRank(Item item)
            {
                ulong rank = 0b_0000000000000000000000000000000000000000000000000000000000000000;
                if (item == null) return rank;

                ulong mask = 0b_1000000000000000000000000000000000000000000000000000000000000000;
                int bitsLeft = 64;

                foreach (Category category in this)
                    switch (category)
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
                        case Category.NULL:
                        default:
                            break;
                    }

                return rank;
            }

            public bool sortItemGrid(ItemGrid itemGrid)
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
        }
    }

    internal static class CategoryMethods
    {
        public static string GetString(this Sort.Category category)
        {
            return category switch
            {
                Sort.Category.UNIQUENESS => "Uniqueness",
                Sort.Category.RARITY => "Rarity",
                Sort.Category.TIER => "Tier",
                Sort.Category.TYPE => "Type",
                Sort.Category.NULL => "null",
                _ => null
            };
        }
    }
}