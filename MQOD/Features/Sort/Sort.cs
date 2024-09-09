using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Death.Items;
using HarmonyLib;
using MelonLoader;

namespace MQOD
{
    public static class Sort
    {
        public static readonly MethodInfo Item__GetUnique =
            typeof(Item).GetProperty(nameof(Item.IsUnique))!.GetGetMethod();

        public static readonly MethodInfo Item__GetRarity =
            typeof(Item).GetProperty(nameof(Item.Rarity))!.GetGetMethod();

        public static readonly MethodInfo Item__GetTier = typeof(Item).GetProperty(nameof(Item.Tier))!.GetGetMethod();
        public static readonly MethodInfo TierId__GetId = typeof(TierId).GetProperty(nameof(TierId.Id))!.GetGetMethod();
        public static readonly MethodInfo Item__GetType = typeof(Item).GetProperty(nameof(Item.Type))!.GetGetMethod();

        public static readonly MethodInfo Item__GetSubtypeCode =
            typeof(Item).GetProperty(nameof(Item.SubtypeCode))!.GetGetMethod();

        public static readonly MethodInfo String__GetChars =
            typeof(string).GetProperty("Chars", new[] { typeof(int) })!.GetGetMethod();

        public static readonly MethodInfo String__GetLength =
            typeof(string).GetProperty(nameof(string.Length))!.GetGetMethod();

        public enum Category
        {
            UNIQUENESS,
            RARITY,
            TIER,
            TYPE,
            NULL
        }

        private static readonly FieldInfo ItemGridSlotsAccessor = typeof(ItemGrid).GetField("_slots", AccessTools.all);

        public delegate TReturn OneParameter<out TReturn, in TParameter0>(TParameter0 p0);

        private static void doILStuff(ILGenerator il)
        {
            List<Label> labels = new();

            il.DeclareLocal(typeof(ulong));
            il.DeclareLocal(typeof(ulong));
            il.DeclareLocal(typeof(uint));
            il.DeclareLocal(typeof(TierId));
            il.DeclareLocal(typeof(string));
            il.DeclareLocal(typeof(int));
            il.DeclareLocal(typeof(int));
            il.DeclareLocal(typeof(int));
            for (int i = 0; i < 8; i++)
            {
                labels.Add(il.DefineLabel());
            }

            il.EmitLog(OpCodes.Ldc_I4_0);
            il.EmitLog(OpCodes.Conv_I8);
            il.EmitLog(OpCodes.Stloc, 0);
            il.EmitLog(OpCodes.Ldarg_0);
            il.EmitLog(OpCodes.Brtrue_S, labels[0]);
            il.EmitLog(OpCodes.Ldloc, 0);
            il.EmitLog(OpCodes.Ret);
            il.MarkLabel(labels[0]);
            il.EmitLog(OpCodes.Ldc_I8, -9223372036854775808);
            il.EmitLog(OpCodes.Stloc, 1);
            il.EmitLog(OpCodes.Ldc_I4_S, 64);
            il.EmitLog(OpCodes.Stloc, 2);
            il.EmitLog(OpCodes.Ldarg_0);
            il.EmitLogCall(OpCodes.Callvirt, Item__GetUnique, null);
            il.EmitLog(OpCodes.Brfalse_S, labels[1]);
            il.EmitLog(OpCodes.Ldloc, 0);
            il.EmitLog(OpCodes.Ldloc, 1);
            il.EmitLog(OpCodes.Or);
            il.EmitLog(OpCodes.Stloc, 0);
            il.MarkLabel(labels[1]);
            il.EmitLog(OpCodes.Ldloc, 1);
            il.EmitLog(OpCodes.Ldc_I4_1);
            il.EmitLog(OpCodes.Shr_Un);
            il.EmitLog(OpCodes.Stloc, 1);
            il.EmitLog(OpCodes.Ldloc, 2);
            il.EmitLog(OpCodes.Ldc_I4_1);
            il.EmitLog(OpCodes.Sub);
            il.EmitLog(OpCodes.Stloc, 2);
            il.EmitLog(OpCodes.Ldloc, 0);
            il.EmitLog(OpCodes.Ldloc, 1);
            il.EmitLog(OpCodes.Ldc_I4_6);
            il.EmitLog(OpCodes.Ldarg_0);
            il.EmitLogCall(OpCodes.Callvirt, Item__GetRarity, null);
            il.EmitLog(OpCodes.Sub);
            il.EmitLog(OpCodes.Ldc_I4_S, 63);
            il.EmitLog(OpCodes.And);
            il.EmitLog(OpCodes.Shr_Un);
            il.EmitLog(OpCodes.Or);
            il.EmitLog(OpCodes.Stloc, 0);
            il.EmitLog(OpCodes.Ldloc, 1);
            il.EmitLog(OpCodes.Ldc_I4_6);
            il.EmitLog(OpCodes.Shr_Un);
            il.EmitLog(OpCodes.Stloc, 1);
            il.EmitLog(OpCodes.Ldloc, 2);
            il.EmitLog(OpCodes.Ldc_I4_6);
            il.EmitLog(OpCodes.Sub);
            il.EmitLog(OpCodes.Stloc, 2);
            il.EmitLog(OpCodes.Ldloc, 0);
            il.EmitLog(OpCodes.Ldloc, 1);
            il.EmitLog(OpCodes.Ldc_I4_5);
            il.EmitLog(OpCodes.Ldarg_0);
            il.EmitLogCall(OpCodes.Callvirt, Item__GetTier, null);
            il.EmitLog(OpCodes.Stloc, 3);
            il.EmitLog(OpCodes.Ldloca_S, 3);
            il.EmitLogCall(OpCodes.Callvirt, TierId__GetId, null);
            il.EmitLog(OpCodes.Sub);
            il.EmitLog(OpCodes.Ldc_I4_S, 63);
            il.EmitLog(OpCodes.And);
            il.EmitLog(OpCodes.Shr_Un);
            il.EmitLog(OpCodes.Or);
            il.EmitLog(OpCodes.Stloc, 0);
            il.EmitLog(OpCodes.Ldloc, 1);
            il.EmitLog(OpCodes.Ldc_I4_5);
            il.EmitLog(OpCodes.Shr_Un);
            il.EmitLog(OpCodes.Stloc, 1);
            il.EmitLog(OpCodes.Ldloc, 2);
            il.EmitLog(OpCodes.Ldc_I4_5);
            il.EmitLog(OpCodes.Sub);
            il.EmitLog(OpCodes.Stloc, 2);
            il.EmitLog(OpCodes.Ldloc, 0);
            il.EmitLog(OpCodes.Ldloc, 1);
            il.EmitLog(OpCodes.Ldarg_0);
            il.EmitLogCall(OpCodes.Callvirt, Item__GetType, null);
            il.EmitLog(OpCodes.Ldc_I4_S, 63);
            il.EmitLog(OpCodes.And);
            il.EmitLog(OpCodes.Shr_Un);
            il.EmitLog(OpCodes.Or);
            il.EmitLog(OpCodes.Stloc, 0);
            il.EmitLog(OpCodes.Ldloc, 1);
            il.EmitLog(OpCodes.Ldc_I4_S, 11);
            il.EmitLog(OpCodes.Shr_Un);
            il.EmitLog(OpCodes.Stloc, 1);
            il.EmitLog(OpCodes.Ldloc, 2);
            il.EmitLog(OpCodes.Ldc_I4_S, 11);
            il.EmitLog(OpCodes.Sub);
            il.EmitLog(OpCodes.Stloc, 2);
            il.EmitLog(OpCodes.Ldloc, 2);
            il.EmitLog(OpCodes.Ldc_I4_S, 26);
            il.EmitLog(OpCodes.Sub);
            il.EmitLog(OpCodes.Stloc, 2);
            il.EmitLog(OpCodes.Ldarg_0);
            il.EmitLogCall(OpCodes.Callvirt, Item__GetSubtypeCode, null);
            il.EmitLog(OpCodes.Stloc_S, 4);
            il.EmitLog(OpCodes.Ldc_I4_0);
            il.EmitLog(OpCodes.Stloc_S, 5);
            il.MarkLabel(labels[7]);
            il.EmitLog(OpCodes.Br_S, labels[2]);
            il.EmitLog(OpCodes.Ldloc_S, 4);
            il.EmitLog(OpCodes.Ldloc_S, 5);
            il.EmitLogCall(OpCodes.Callvirt, String__GetChars, null);
            il.EmitLog(OpCodes.Stloc_S, 6);
            il.EmitLog(OpCodes.Ldloc_S, 6);
            il.EmitLog(OpCodes.Stloc_S, 7); 
            il.EmitLog(OpCodes.Ldloc_S, 7);
            il.EmitLog(OpCodes.Ldc_I4_S, 65);
            il.EmitLog(OpCodes.Bge_S, labels[3]);
            il.MarkLabel(labels[6]);
            il.EmitLog(OpCodes.Br_S, labels[4]);
            il.EmitLog(OpCodes.Ldloc_S, 7);
            il.EmitLog(OpCodes.Ldc_I4_S, 96);
            il.EmitLog(OpCodes.Ble_S, labels[4]);
            il.EmitLog(OpCodes.Ldloc_S, 7);
            il.EmitLog(OpCodes.Ldc_I4_S, 123);
            il.EmitLog(OpCodes.Blt_S, labels[5]);
            il.EmitLog(OpCodes.Br_S, labels[4]);
            il.MarkLabel(labels[3]);
            il.EmitLog(OpCodes.Ldloc_S, 6);
            il.EmitLog(OpCodes.Ldc_I4_S, 65);
            il.EmitLog(OpCodes.Ldloc, 2);
            il.EmitLog(OpCodes.Add);
            il.EmitLog(OpCodes.Bgt_S, labels[6]);
            il.EmitLog(OpCodes.Ldloc, 0);
            il.EmitLog(OpCodes.Ldloc, 1);
            il.EmitLog(OpCodes.Ldloc_S, 6);
            il.EmitLog(OpCodes.Ldc_I4_S, 65);
            il.EmitLog(OpCodes.Sub);
            il.EmitLog(OpCodes.Ldc_I4_S, 63);
            il.EmitLog(OpCodes.And);
            il.EmitLog(OpCodes.Shr_Un);
            il.EmitLog(OpCodes.Xor);
            il.EmitLog(OpCodes.Stloc, 0);
            il.EmitLog(OpCodes.Br_S, labels[2]);
            il.MarkLabel(labels[5]);
            il.EmitLog(OpCodes.Ldloc, 0);
            il.EmitLog(OpCodes.Ldloc, 1);
            il.EmitLog(OpCodes.Ldloc_S, 6);
            il.EmitLog(OpCodes.Ldc_I4_S, 97);
            il.EmitLog(OpCodes.Sub);
            il.EmitLog(OpCodes.Ldloc, 2);
            il.EmitLog(OpCodes.Add);
            il.EmitLog(OpCodes.Ldc_I4_S, 63);
            il.EmitLog(OpCodes.And);
            il.EmitLog(OpCodes.Shr_Un);
            il.EmitLog(OpCodes.Xor);
            il.EmitLog(OpCodes.Stloc, 0);
            il.MarkLabel(labels[4]);
            il.EmitLog(OpCodes.Ldloc_S, 5);
            il.EmitLog(OpCodes.Ldc_I4_1);
            il.EmitLog(OpCodes.Add);
            il.EmitLog(OpCodes.Stloc_S, 5);
            il.MarkLabel(labels[2]);
            il.EmitLog(OpCodes.Ldloc_S, 5);
            il.EmitLog(OpCodes.Ldloc_S, 4);
            il.EmitLogCall(OpCodes.Callvirt, String__GetLength, null);
            // You don't know how many hours I have wasted on this...
            // il.Emit(OpCodes.Blt_S, labels[7]); is the opcode that is in the compiled binary
            // But ILGenerator can't produce Blt_S with the label??
            // il.EmitLog(OpCodes.Blt, labels[7]);
            il.EmitLog(OpCodes.Clt);
            il.EmitLog(OpCodes.Brtrue, labels[7]);
            il.EmitLog(OpCodes.Ldloc, 0);
            il.EmitLog(OpCodes.Ret);
        }


        public static void GenerateILGetRankIL()
        {
            DynamicMethod dynamicMethod = new DynamicMethod("getRank",
                MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(ulong),
                new[] { typeof(Item) }, typeof(Sort), false);
            // DynamicMethod dynamicMethod = new(
            //     "getRank",
            //     typeof(ulong),
            //     new[] { typeof(Item) }
            // );
            ILGenerator il = dynamicMethod.GetILGenerator(1024 * 4);

            // AssemblyName assemblyName = new AssemblyName("peanits");
            // AssemblyBuilder assembly = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            // ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(assemblyName.Name, "peanits.dll", true);
            // TypeBuilder typeBuilder = moduleBuilder.DefineType("MyClass", TypeAttributes.Public | TypeAttributes.Class);
            // MethodBuilder m = typeBuilder.DefineMethod("ShitMethod", MethodAttributes.Public | MethodAttributes.Static,
            //     CallingConventions.Standard, typeof(ulong),
            //     new[] { typeof(Item) });
            // ILGenerator il = m.GetILGenerator(1024*4);

            doILStuff(il);

            // typeBuilder.CreateType();

            // assembly.Save("peanits.dll", PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
            // m.Invoke(null, BindingFlags.Public | BindingFlags.Static, null, new object[]{new Item("ShitItem", ItemType.Amulet, ItemClass.None, ItemRarity.Broken, TierId.FromId(1), false, "shit",
            //     "balls", "ass", new List<Item.AffixReference>())}, null);

            dynamicMethod.Invoke(null, new[]
            {
                new Item("ShitItem", ItemType.Amulet, ItemClass.None, ItemRarity.Broken, TierId.FromId(1), false,
                    "shit",
                    "balls", "ass", new List<Item.AffixReference>())
            });
            // OneParameter<ulong, Item> del =
            //     (OneParameter<ulong, Item>)dynamicMethod.CreateDelegate(typeof(OneParameter<ulong, Item>));
            // MelonLogger.Msg("del: " + del);
            return;
            // OneParameter<ulong, Item> getRankIL =
            //     (OneParameter<ulong, Item>)dynamicMethod.CreateDelegate(typeof(OneParameter<ulong, Item>));
            // return del;
        }

        public static ulong getRank(Item item)
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