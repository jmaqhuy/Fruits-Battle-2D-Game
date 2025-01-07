using System.Collections.Generic;

namespace DataTransfer
{
    public static class RankStatic
    {
        public static readonly List<RankModel> RankModels = new List<RankModel>
        {
            new RankModel()
            {
                Id = 1,
                Name = "Bronze",
                AssetName = "Bronze",
                MaxStar = 3
            },
            new RankModel()
            {
                Id = 2,
                Name = "Silver",
                AssetName = "Silver",
                MaxStar = 3
            },
            new RankModel()
            {
                Id = 3,
                Name = "Gold",
                AssetName = "Gold",
                MaxStar = 4
            },
            new RankModel()
            {
                Id = 4,
                Name = "Majestic",
                AssetName = "Majestic",
                MaxStar = 5
            },
            new RankModel()
            {
                Id = 5,
                Name = "Legendary",
                AssetName = "Legendary",
                MaxStar = 0
            },
        };
    }

    public class RankModel
    {
        public int Id { get; set; }
        public int MaxStar { get; set; }
        public string Name { get; set; }
        public string AssetName { get; set; }
    }

}
