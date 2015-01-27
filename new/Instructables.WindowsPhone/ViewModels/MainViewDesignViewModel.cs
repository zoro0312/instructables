using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.DataModel;
using Instructables.DataServices;

namespace Instructables.ViewModels
{
    class MainViewDesignViewModel : ObservableCollection<DataGroup>
    {
        public MainViewDesignViewModel()
        {
            InitializeData();
        }

        private async void InitializeData()
        {
            var dataService = new InstructablesDataService();
            int ColumnHeight = 3;
            var recentGroup = new DataGroup() { GroupOrdinal = 0, GroupName = "Recent", Layout = DataGroup.LayoutType.MainFeature };
            var recent = await dataService.GetRecent(limit: ColumnHeight + 1);
            int count = 0;
            foreach (var item in recent.items)
            {
                item.GroupOrdinal = count++;
                item.Group = recentGroup;
                recentGroup.GroupItems.Add(item);
                if (count > ColumnHeight)
                    break;
            }
            this.Add(recentGroup);

            var featuredGroup = new DataGroup() { GroupOrdinal = 1, GroupName = "Featured", Layout = DataGroup.LayoutType.DualFeature };
            var featured = await dataService.GetFeatured(limit: ColumnHeight + 2);
            count = 0;
            foreach (var item in featured.items)
            {
                item.GroupOrdinal = count++;
                item.Group = featuredGroup;
                featuredGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 1)
                    break;
            }
            this.Add(featuredGroup);

            var popularGroup = new DataGroup() { GroupOrdinal = 2, GroupName = "Popular", Layout = DataGroup.LayoutType.DualFeature };
            var popular = await dataService.GetPopular(limit: ColumnHeight + 2);
            count = 0;
            foreach (var item in popular.items)
            {
                item.GroupOrdinal = count++;
                item.Group = popularGroup;
                popularGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 1)
                    break;
            }
            this.Add(popularGroup);

            var eBooksGroup = new DataGroup() { GroupOrdinal = 3, GroupName = "eBooks", Layout = DataGroup.LayoutType.EBook };
            var ebooks = await dataService.GeteBooks(limit: ColumnHeight + 3);
            count = 0;
            foreach (var item in ebooks.items)
            {
                item.GroupOrdinal = count++;
                item.Group = eBooksGroup;
                eBooksGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 2)
                    break;
            }
            this.Add(eBooksGroup);

            var technologyGroup = new DataGroup() { GroupOrdinal = 4, GroupName = "Technology", Layout = DataGroup.LayoutType.SingleFeature };
            var technology = await dataService.GetTechnology(limit: ColumnHeight + 3);
            count = 0;
            foreach (var item in technology.items)
            {
                item.GroupOrdinal = count++;
                item.Group = technologyGroup;
                technologyGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 2)
                    break;
            }
            this.Add(technologyGroup);

            var workshopGroup = new DataGroup() { GroupOrdinal = 5, GroupName = "Workshop", Layout = DataGroup.LayoutType.DualFeature };
            var workshop = await dataService.GetWorkshop(limit: ColumnHeight + 2);
            count = 0;
            foreach (var item in workshop.items)
            {
                item.GroupOrdinal = count++;
                item.Group = workshopGroup;
                workshopGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 1)
                    break;
            }
            this.Add(workshopGroup);

            var livingGroup = new DataGroup() { GroupOrdinal = 6, GroupName = "Living", Layout = DataGroup.LayoutType.SingleFeature };
            var living = await dataService.GetLiving(limit: ColumnHeight + 3);
            count = 0;
            foreach (var item in living.items)
            {
                item.GroupOrdinal = count++;
                item.Group = livingGroup;
                livingGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 2)
                    break;
            }
            this.Add(livingGroup);

            var foodGroup = new DataGroup() { GroupOrdinal = 7, GroupName = "Food", Layout = DataGroup.LayoutType.DualFeature };
            var food = await dataService.GetFood(limit: ColumnHeight + 2);
            count = 0;
            foreach (var item in food.items)
            {
                item.GroupOrdinal = count++;
                item.Group = foodGroup;
                foodGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 1)
                    break;
            }
            this.Add(foodGroup);

            var playGroup = new DataGroup() { GroupOrdinal = 8, GroupName = "Play", Layout = DataGroup.LayoutType.SingleFeature };
            var play = await dataService.GetPlay(limit: ColumnHeight + 3);
            count = 0;
            foreach (var item in play.items)
            {
                item.GroupOrdinal = count++;
                item.Group = playGroup;
                playGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 2)
                    break;
            }
            this.Add(playGroup);

            var outsideGroup = new DataGroup() { GroupOrdinal = 9, GroupName = "Outside", Layout = DataGroup.LayoutType.DualFeature };
            var outside = await dataService.GetOutside(limit: ColumnHeight + 2);
            count = 0;
            foreach (var item in outside.items)
            {
                item.GroupOrdinal = count++;
                item.Group = outsideGroup;
                outsideGroup.GroupItems.Add(item);
                if (count > ColumnHeight + 1)
                    break;
            }
            this.Add(outsideGroup);
        }
    }
}
