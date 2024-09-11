using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Death.Items;
using HarmonyLib;

namespace MQOD
{
    public static class Sort
    {
        public delegate ulong CalcDelegate(bool IsUnique, int Rarity, int Tier, int Type, string SubType);

        public enum Category
        {
            UNIQUENESS,
            RARITY,
            TIER,
            TYPE,
            NULL
        }

        private const int uniqueArg = 0;
        private const int rarityArg = 1;
        private const int tierIdArg = 2;
        private const int typeArg = 3;
        private const int subtypeCodeArg = 4;

        public static readonly MethodInfo String__GetChars =
            typeof(string).GetProperty("Chars", new[] { typeof(int) })!.GetGetMethod();

        public static readonly MethodInfo String__GetLength =
            typeof(string).GetProperty(nameof(string.Length))!.GetGetMethod();

        public static CalcDelegate currentCalcDelegate;

        private static readonly FieldInfo ItemGridSlotsAccessor = typeof(ItemGrid).GetField("_slots", AccessTools.all);

        public static CalcDelegate GenerateCalcDelegate(Ordering ordering)
        {
            // MelonLogger.Msg("Generating new CalcDelegate");
            DynamicMethod dynamicMethod = new(
                "calcIL", typeof(ulong), new[] { typeof(bool), typeof(int), typeof(int), typeof(int), typeof(string) });

            ILGenerator il = dynamicMethod.GetILGenerator(512);
            buildCalcIL(il, ordering);
            return (CalcDelegate)dynamicMethod.CreateDelegate(typeof(CalcDelegate));

            [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
            static void buildCalcIL(ILGenerator il, Ordering ordering)
            {
                Label[] l = new Label[8];
                for (int i = 0; i < l.Length; i++) l[i] = il.DefineLabel();
                LocalBuilder rank = il.DeclareLocal(typeof(ulong)); // 0 local 
                LocalBuilder mask = il.DeclareLocal(typeof(ulong)); // 1
                LocalBuilder bitsLeft = il.DeclareLocal(typeof(int)); // 2
                LocalBuilder stringTmp = il.DeclareLocal(typeof(string)); // 3
                LocalBuilder someInt4 = il.DeclareLocal(typeof(int)); // 4
                LocalBuilder someInt5 = il.DeclareLocal(typeof(int)); // 5
                LocalBuilder someInt6 = il.DeclareLocal(typeof(int)); // 6 

                // ulong num = 0uL;
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Conv_I8);
                il.Emit(OpCodes.Stloc, rank.LocalIndex);
                // ulong num2 = 9223372036854775808uL;
                il.Emit(OpCodes.Ldc_I8, -9223372036854775808);
                il.Emit(OpCodes.Stloc, mask.LocalIndex);
                // int num3 = 64;
                il.Emit(OpCodes.Ldc_I4_S, 64);
                il.Emit(OpCodes.Stloc, bitsLeft.LocalIndex);
                foreach (Category category in ordering)
                    switch (category)
                    {
                        case Category.UNIQUENESS:
                            // if (IsUnique)
                            il.Emit(OpCodes.Ldarg, uniqueArg);
                            il.Emit(OpCodes.Brfalse_S, l[0]);
                            // num |= num2;
                            il.Emit(OpCodes.Ldloc, rank.LocalIndex);
                            il.Emit(OpCodes.Ldloc, mask.LocalIndex);
                            il.Emit(OpCodes.Or);
                            il.Emit(OpCodes.Stloc, rank.LocalIndex);
                            // num2 >>= 1;
                            il.MarkLabel(l[0]);
                            il.Emit(OpCodes.Ldloc, mask.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_1);
                            il.Emit(OpCodes.Shr_Un);
                            il.Emit(OpCodes.Stloc, mask.LocalIndex);
                            // num3--;
                            il.Emit(OpCodes.Ldloc, bitsLeft.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_1);
                            il.Emit(OpCodes.Sub);
                            il.Emit(OpCodes.Stloc, bitsLeft.LocalIndex);
                            break;
                        case Category.RARITY:
                            // num |= num2 >> itemRarityCount - itemRarity;
                            il.Emit(OpCodes.Ldloc, rank.LocalIndex);
                            il.Emit(OpCodes.Ldloc, mask.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4, (int)ItemRarity._Count);
                            il.Emit(OpCodes.Ldarg, rarityArg);
                            il.Emit(OpCodes.Sub);
                            il.Emit(OpCodes.Shr_Un);
                            il.Emit(OpCodes.Or);
                            il.Emit(OpCodes.Stloc, rank.LocalIndex);
                            // num2 >>= itemRarityCount;
                            il.Emit(OpCodes.Ldloc, mask.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4, (int)ItemRarity._Count);
                            il.Emit(OpCodes.Shr_Un);
                            il.Emit(OpCodes.Stloc, mask.LocalIndex);
                            // num3 -= itemRarityCount;
                            il.Emit(OpCodes.Ldloc, bitsLeft.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4, (int)ItemRarity._Count);
                            il.Emit(OpCodes.Sub);
                            il.Emit(OpCodes.Stloc, bitsLeft.LocalIndex);
                            break;
                        case Category.TIER:
                            // num |= num2 >> tierIdCount - itemTierId;
                            il.Emit(OpCodes.Ldloc, rank.LocalIndex);
                            il.Emit(OpCodes.Ldloc, mask.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4, TierId.Count);
                            il.Emit(OpCodes.Ldarg, tierIdArg);
                            il.Emit(OpCodes.Sub);
                            il.Emit(OpCodes.Shr_Un);
                            il.Emit(OpCodes.Or);
                            il.Emit(OpCodes.Stloc, rank.LocalIndex);
                            // num2 >>= tierIdCount;
                            il.Emit(OpCodes.Ldloc, mask.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4, TierId.Count);
                            il.Emit(OpCodes.Shr_Un);
                            il.Emit(OpCodes.Stloc, mask.LocalIndex);
                            // num3 -= tierIdCount;
                            il.Emit(OpCodes.Ldloc, bitsLeft.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4, TierId.Count);
                            il.Emit(OpCodes.Sub);
                            il.Emit(OpCodes.Stloc, bitsLeft.LocalIndex);
                            break;
                        case Category.TYPE:
                            // num |= num2 >> itemType;
                            il.Emit(OpCodes.Ldloc, rank.LocalIndex);
                            il.Emit(OpCodes.Ldloc, mask.LocalIndex);
                            il.Emit(OpCodes.Ldarg, typeArg);
                            il.Emit(OpCodes.Shr_Un);
                            il.Emit(OpCodes.Or);
                            il.Emit(OpCodes.Stloc, rank.LocalIndex);
                            // num2 >>= itemTypeCount;
                            il.Emit(OpCodes.Ldloc, mask.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_S, (int)ItemType._Count);
                            il.Emit(OpCodes.Shr_Un);
                            il.Emit(OpCodes.Stloc, mask.LocalIndex);
                            // num3 -= itemTypeCount;
                            il.Emit(OpCodes.Ldloc, bitsLeft.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_S, (int)ItemType._Count);
                            il.Emit(OpCodes.Sub);
                            il.Emit(OpCodes.Stloc, bitsLeft.LocalIndex);
                            // num3 -= 26;
                            il.Emit(OpCodes.Ldloc, bitsLeft.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_S, 26);
                            il.Emit(OpCodes.Sub);
                            il.Emit(OpCodes.Stloc, bitsLeft.LocalIndex);
                            // foreach
                            il.Emit(OpCodes.Ldarg_S, subtypeCodeArg);
                            il.Emit(OpCodes.Stloc, stringTmp.LocalIndex);
                            // (no C# code)
                            il.Emit(OpCodes.Ldc_I4_0);
                            il.Emit(OpCodes.Stloc_S, someInt4.LocalIndex);
                            il.Emit(OpCodes.Br, l[6]);
                            // loop start (head: IL_00c2)
                            // int num5 = num4;
                            il.MarkLabel(l[1]);
                            il.Emit(OpCodes.Ldloc, stringTmp.LocalIndex);
                            il.Emit(OpCodes.Ldloc_S, someInt4.LocalIndex);
                            il.EmitCall(OpCodes.Callvirt, String__GetChars, null);
                            il.Emit(OpCodes.Stloc_S, someInt5.LocalIndex);
                            il.Emit(OpCodes.Ldloc_S, someInt5.LocalIndex);
                            il.Emit(OpCodes.Stloc_S, someInt6.LocalIndex);
                            // if (num5 < 65)
                            il.Emit(OpCodes.Ldloc_S, someInt6.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_S, 65);
                            il.Emit(OpCodes.Bge_S, l[3]);
                            // if (num5 > 96 && num5 < 123)
                            il.Emit(OpCodes.Br, l[4]);
                            // loop start (head: IL_0096)
                            il.MarkLabel(l[2]);
                            il.Emit(OpCodes.Ldloc_S, someInt6.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_S, 96);
                            il.Emit(OpCodes.Ble_S, l[4]);
                            il.Emit(OpCodes.Ldloc_S, someInt6.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_S, 123);
                            il.Emit(OpCodes.Clt);
                            il.Emit(OpCodes.Brtrue, l[5]);
                            // if (num4 > 65 + num3)
                            il.Emit(OpCodes.Br_S, l[4]);
                            il.MarkLabel(l[3]);
                            il.Emit(OpCodes.Ldloc_S, someInt5.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_S, 65);
                            il.Emit(OpCodes.Ldloc, bitsLeft.LocalIndex);
                            il.Emit(OpCodes.Add);
                            il.Emit(OpCodes.Bgt_S, l[2]);
                            // end loop
                            // num ^= num2 >> num4 - 65;
                            il.Emit(OpCodes.Ldloc, rank.LocalIndex);
                            il.Emit(OpCodes.Ldloc, mask.LocalIndex);
                            il.Emit(OpCodes.Ldloc_S, someInt5.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_S, 65);
                            il.Emit(OpCodes.Sub);
                            il.Emit(OpCodes.Shr_Un);
                            il.Emit(OpCodes.Xor);
                            il.Emit(OpCodes.Stloc, rank.LocalIndex);
                            // num ^= num2 >> num4 - 97 + num3;
                            il.Emit(OpCodes.Br_S, l[4]);
                            il.MarkLabel(l[5]);
                            il.Emit(OpCodes.Ldloc, rank.LocalIndex);
                            il.Emit(OpCodes.Ldloc, mask.LocalIndex);
                            il.Emit(OpCodes.Ldloc_S, someInt5.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_S, 97);
                            il.Emit(OpCodes.Sub);
                            il.Emit(OpCodes.Ldloc, bitsLeft.LocalIndex);
                            il.Emit(OpCodes.Add);
                            il.Emit(OpCodes.Shr_Un);
                            il.Emit(OpCodes.Xor);
                            il.Emit(OpCodes.Stloc, rank.LocalIndex);
                            il.MarkLabel(l[4]);
                            il.Emit(OpCodes.Ldloc_S, someInt4.LocalIndex);
                            il.Emit(OpCodes.Ldc_I4_1);
                            il.Emit(OpCodes.Add);
                            il.Emit(OpCodes.Stloc_S, someInt4.LocalIndex);
                            // return num;
                            il.MarkLabel(l[6]);
                            il.Emit(OpCodes.Ldloc_S, someInt4.LocalIndex);
                            il.Emit(OpCodes.Ldloc, stringTmp.LocalIndex);
                            il.EmitCall(OpCodes.Callvirt, String__GetLength, null);
                            il.Emit(OpCodes.Clt);
                            il.Emit(OpCodes.Brtrue, l[1]);
                            // end loop
                            break;
                        case Category.NULL:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                il.Emit(OpCodes.Ldloc, rank.LocalIndex);
                il.Emit(OpCodes.Ret);
            }
        }

        public static void SortArrayInPlace(ulong[] array, int leftIndex, int rightIndex) // QuickSort 
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

        public static bool sortItemGrid(ItemGrid itemGrid)
        {
            Dictionary<ulong, List<Item>> ItemRank = new();

            foreach (Item item in GetItemsWithNulls(itemGrid))
            {
                ulong rank = 0;
                if (item != null)
                    rank = currentCalcDelegate(item.IsUnique, (int)item.Rarity, item.Tier.Id, (int)item.Type,
                        item.SubtypeCode);
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