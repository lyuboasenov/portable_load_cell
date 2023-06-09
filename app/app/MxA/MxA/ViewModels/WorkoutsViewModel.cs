﻿using MxA.Database.Models;
using MxA.Models;
using MxA.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MxA.ViewModels {
   public class WorkoutsViewModel : BaseViewModel {
      public ObservableCollection<Models.WorkoutType> Items { get; }
      public Command LoadItemsCommand { get; }
      public Command AddItemCommand { get; }
      public Command<Workout> ItemTapped { get; }
      public string SearchTerm { get; set; }

      public WorkoutsViewModel() {
         Title = "Workouts";
         Items = new ObservableCollection<Models.WorkoutType>();
         LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

         ItemTapped = new Command<Workout>(this.OnItemSelected);

         AddItemCommand = new Command(OnAddItem);

         OnAppearing();
      }

      public void OnSearchTermChanged() {
         IsRefreshingData = true;
      }

      async Task ExecuteLoadItemsCommand() {
         IsRefreshingData = true;

         try {
            Items.Clear();

            var types = await DataStore.Types.GetItemsAsync();
            types = types.OrderBy(t => t.Order);

            var targets = await DataStore.Targets.GetItemsAsync();
            var workouts = await DataStore.Workouts.GetItemsAsync();
            workouts = workouts.Where(w => string.IsNullOrEmpty(SearchTerm) || w.Name.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
            var progressions = await DataStore.Progression.GetItemsAsync();

            foreach (var type in types) {
               var list = new List<WorkoutTarget>();
               foreach(var target in targets) {
                  var filteredWorkouts = workouts.Where(w => w.TypeId == type.Id && w.TargetId == target.Id && w.WorkoutList);
                  if (filteredWorkouts.Any()) {
                     list.Add(new WorkoutTarget(target, filteredWorkouts.OrderBy(w => w.Name)));
                  }
               }
               if (list.Any()) {
                  Items.Add(new WorkoutType(type, list));
               }
            }
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         } finally {
            IsRefreshingData = false;
         }
      }

      public void OnAppearing() {
         IsRefreshingData = true;
         SelectedItem = null;
      }

      public Workout SelectedItem { get; set; }

      public void OnSelectedItemChanged() {
         OnItemSelected(SelectedItem);
      }

      private async void OnAddItem(object obj) {
         await Shell.Current.GoToAsync(nameof(NewItemPage));
      }

      async void OnItemSelected(Workout item) {
         if (item == null)
            return;

         // This will push the ItemDetailPage onto the navigation stack
         await Shell.Current.GoToAsync($"//{nameof(TrainingsPage)}/{nameof(WorkoutPage)}?{nameof(WorkoutViewModel.WorkoutId)}={item.Id}");
      }
   }
}