using System.Collections.Generic;
using Death.Items;

namespace MQOD
{
    public class Archive
    {
        public class Ordering : List<Sort.Category>
        {
            private ulong getRank(Item item)
            {
                ulong rank = 0b_0000000000000000000000000000000000000000000000000000000000000000;
                if (item == null) return rank;

                ulong mask = 0b_1000000000000000000000000000000000000000000000000000000000000000;
                int bitsLeft = 64;

                foreach (Sort.Category category in this)
                    switch (category)
                    {
                        case global::MQOD.Sort.Category.UNIQUENESS:
                            if (item.IsUnique) rank |= mask;
                            mask >>= 1;
                            bitsLeft -= 1;
                            break;
                        case global::MQOD.Sort.Category.RARITY:
                            rank |= mask >> ((int)ItemRarity._Count - (int)item.Rarity);
                            mask >>= (int)ItemRarity._Count;
                            bitsLeft -= (int)ItemRarity._Count;
                            break;
                        case global::MQOD.Sort.Category.TIER:
                            rank |= mask >> (TierId.Count - item.Tier.Id);
                            mask >>= TierId.Count;
                            bitsLeft -= TierId.Count;
                            break;
                        case global::MQOD.Sort.Category.TYPE:
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
                        case global::MQOD.Sort.Category.NULL:
                        default:
                            break;
                    }

                return rank;
            }
        }
    }
}