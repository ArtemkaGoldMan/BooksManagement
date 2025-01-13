using BaseLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiClient.ViewModel
{
    public partial class BorrowHistoryViewModel : ObservableObject
    {
        private readonly BorrowHistoryService _borrowHistoryService;

        private const int PageSize = 10;

        [ObservableProperty]
        private List<BorrowHistoryDTO> borrowHistory;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private int currentPage;

        [ObservableProperty]
        private bool canGoPrevious;

        [ObservableProperty]
        private bool canGoNext;

        public BorrowHistoryViewModel(BorrowHistoryService borrowHistoryService)
        {
            _borrowHistoryService = borrowHistoryService;
            CurrentPage = 1;
            BorrowHistory = new List<BorrowHistoryDTO>();
        }

        [RelayCommand]
        public async Task LoadAllHistoryAsync()
        {
            await LoadHistoryAsync(() => _borrowHistoryService.GetBorrowHistoryAsync());
        }

        [RelayCommand]
        public async Task LoadNotReturnedBooksAsync()
        {
            await LoadHistoryAsync(() => _borrowHistoryService.GetNotReturnedBooksAsync());
        }

        [RelayCommand]
        public async Task NextPageAsync()
        {
            CurrentPage++;
            await LoadAllHistoryAsync();
        }

        [RelayCommand]
        public async Task PreviousPageAsync()
        {
            CurrentPage--;
            await LoadAllHistoryAsync();
        }

        private async Task LoadHistoryAsync(Func<Task<List<BorrowHistoryDTO>>> fetchHistoryFunc)
        {
            try
            {
                var allHistory = await fetchHistoryFunc();
                BorrowHistory = allHistory
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                CanGoPrevious = CurrentPage > 1;
                CanGoNext = allHistory.Count > CurrentPage * PageSize;
                Message = null;
            }
            catch (Exception ex)
            {
                Message = $"Error loading history: {ex.Message}";
            }
        }
    }
}
