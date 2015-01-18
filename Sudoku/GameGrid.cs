using System;
using Xamarin.Forms;
using Button = Xamarin.Forms.Button;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Timer = System.Timers.Timer;


/// <summary>
/// Author: Anthony Simmons (AnthonySimmons99@gmail.com)
/// October 2014
/// Description: Game Page used to display the user interface, including
/// the Game Control buttons, difficulty selector, Cell grid buttons, Number selection buttons
/// Clear buttons and Progress bar.
/// </summary>
namespace Sudoku
{
	public class GameGrid : ContentPage
	{
		int WrongChoice = -1;

		int pageWidth;
		int pageHeight;

		int btnWidth = 35;
		int btnHeight = 35;

		Sudoku sudoku;


		List<Button> ButtonList = new List<Button>();
		List<Button> SelectionButtonList = new List<Button>();
		List<BoxView> BackBoxes = new List<BoxView>();

		Picker diffPicker = new Picker ();
		Button newGameBtn = new Button();
		Button hintBtn = new Button ();
		Button solveBtn = new Button();
		Button undoBtn = new Button();
		BoxView mBox = new BoxView();
		ActivityIndicator act = new ActivityIndicator();

		ProgressBar progressBar = new ProgressBar();

		BoxView selectionBox = new BoxView();


		Button ClearAll = new Button();
		Button ClearSelected = new Button();

		Label timerLabel = new Label();

		Label titleLabel = new Label ();

		Timer timer = new Timer();

		public StackLayout layout = new StackLayout ();

		Grid grid = new Grid ();

		BoxView horizontalBox = new BoxView ();

		BoxView blackBox = new BoxView ();


		private GameLog gameLog;


		public bool IsVertical
		{
			get{ return pageHeight >= pageWidth; }
		}


		public GameGrid (Sudoku mSudoku, GameLog mGameLog, int width, int height)
		{
			Title = "Sudoku";
			sudoku = mSudoku;
			gameLog = mGameLog;

			UpdateScreenSize (width, height);

			timer.Interval = 1000;
			timer.Elapsed += TimerTicked;

			if (!sudoku.IsComplete () || !sudoku.IsValid ()) 
			{
				timer.Start ();
			}
			else 
			{
				timer.Stop ();
			}

			this.BackgroundColor = Color.FromRgb(0, 64, 128);
			
		}



		public void Dispose()
		{
			timer.Stop ();
			timer.Elapsed -= TimerTicked;

			timer = null;
			diffPicker = null;
			newGameBtn = null;
			hintBtn = null;
			solveBtn = null;
			undoBtn = null;
			mBox = null;
			act = null;

			selectionBox = null;
			horizontalBox = null;
			blackBox = null;

			titleLabel = null;
			layout = null;

			progressBar = null;

			gameLog = null;

			ClearAll = null;
			ClearSelected = null;


			for(int i = 0; i < ButtonList.Count; i++)
			{
				ButtonList[i] = null;
			}
			ButtonList.Clear ();
			ButtonList = null;

			for (int i = 0; i < SelectionButtonList.Count; i++) 
			{
				SelectionButtonList [i] = null;
			}
			SelectionButtonList.Clear ();
			SelectionButtonList = null;

			for (int i = 0; i < BackBoxes.Count; i++) 
			{
				BackBoxes [i] = null;
			}
			BackBoxes.Clear ();
			BackBoxes = null;

			GC.Collect ();
		}

		public void UpdateScreenSize(int width, int height)
		{
			pageHeight = height;
			pageWidth = width;

			btnWidth = btnHeight = (int)((Math.Min(width, height)) / 10.0);

			if (!IsVertical) 
			{
				btnWidth = btnHeight = (int)((Math.Min(width, height)) / 10.0) - 3;
			}

			CreateLayout ();
			//UpdateButtons ();
		}


		private void CreateControls(Grid grid)
		{
			/*progressBar = new ProgressBar ();
			diffPicker = new Picker ();
			newGameBtn = new Button ();
			hintBtn = new Button ();
			solveBtn = new Button ();
			undoBtn = new Button ();
			mBox = new BoxView ();
			act = new ActivityIndicator ();
			timerLabel = new Label ();*/

			progressBar.IsVisible = false;
			progressBar.HeightRequest = btnHeight / 2;
			progressBar.WidthRequest = btnWidth * 9;

			timerLabel.XAlign = TextAlignment.Center;
			timerLabel.Text = sudoku.TimeString;
			timerLabel.Font = Font.SystemFontOfSize (btnHeight-10);

			diffPicker.Items.Add ("Easy");
			diffPicker.Items.Add ("Medium");
			diffPicker.Items.Add ("Hard");
			diffPicker.SelectedIndexChanged += DifficultyIndexChange;
			diffPicker.SelectedIndex = diffPicker.Items.IndexOf (sudoku.difficulty.ToString());
			diffPicker.Title = diffPicker.Items [diffPicker.SelectedIndex].ToString ();
			diffPicker.HeightRequest = btnHeight;


			newGameBtn.Text = "New";
			newGameBtn.HeightRequest = btnHeight;
			newGameBtn.Clicked += NewGameButtonClick;


			hintBtn.Text = "Hint";
			hintBtn.IsEnabled = true;
			hintBtn.HeightRequest = btnHeight;
			hintBtn.Clicked += HintButtonClick;

			solveBtn.Text = "Solve";
			solveBtn.IsEnabled = true;
			solveBtn.HeightRequest = btnHeight;
			solveBtn.Clicked += SolveButtonClick;

			mBox.HeightRequest = btnHeight;
			mBox.WidthRequest = btnWidth;

			act.IsVisible = false;
			act.IsRunning = false;

			if (IsVertical) 
			{
				grid.Children.Add (newGameBtn, 0, 2, 1, 2);
				grid.Children.Add (diffPicker, 2, 5, 1, 2);
				grid.Children.Add (hintBtn, 5, 7, 1, 2);
				grid.Children.Add (solveBtn, 7, 9, 1, 2);
				grid.Children.Add (act, 0, 2, 0, 1);
				grid.Children.Add (timerLabel, 3, 6, 2, 3); 

				grid.Children.Add (progressBar, 0, 9, 13, 14);
			}
			else
			{
				//horizontalBox = new BoxView ();
				grid.Children.Add (horizontalBox, 1, 2, 1, 10);
				grid.Children.Add (newGameBtn, 0, 1, 2, 3);
				grid.Children.Add (diffPicker, 0, 1, 3, 4);
				grid.Children.Add (hintBtn, 0, 1, 4, 5);
				grid.Children.Add (solveBtn, 0, 1, 5, 6);
				grid.Children.Add (act, 0, 1, 2, 3);
				grid.Children.Add (timerLabel, 0, 1, 6, 7);

				grid.Children.Add (progressBar, 3, 12, 10, 11);
			}

			grid.Children.Add (mBox, 0, 2);
		}

		private void CreateGridButtons(Grid grid)
		{
			ButtonList.Clear ();
			for (int i = 0; i < 9; i++)
			{	
				for (int j = 0; j < 9; j++)
				{
					Button btn = new Button ()
					{
						ClassId = "Cell_" + i.ToString () + "_" + j.ToString () + "_t",
						WidthRequest = btnWidth,
						HeightRequest = btnHeight,
						BackgroundColor = Color.White,
					};


					if (sudoku.Cells [i, j].Value != 0 && sudoku.Cells [i, j].ReadOnly) 
					{
						btn.BackgroundColor = Color.Silver;
					}

					if (sudoku.Cells [i, j].Highlight) 
					{
						btn.TextColor = Color.Green;
					}
					else 
					{
						btn.TextColor = Color.Black;
					}
					if (sudoku.SelectedCell != null && 
						sudoku.Cells [i, j].Value == sudoku.SelectedCell.Value) 
					{
						btn.TextColor = Color.Blue;
					}
					if (sudoku.SelectedCell != null && 
						sudoku.SelectedCell.Row == i && sudoku.SelectedCell.Column == j) 
					{
						btn.BackgroundColor = Color.Gray;
					}

					if (sudoku.Cells [i, j].Value != 0) 
					{
						btn.Text = sudoku.Cells [i, j].Value.ToString ();
					}
					else
					{
						btn.Text = String.Empty;
					}

					ButtonList.Add (btn);

					//btn.BindingContext = sudoku.Cells [i, j];
					//btn.SetBinding (Button.TextProperty, "Value", BindingMode.OneWay);

					if (IsVertical) 
					{
						grid.Children.Add (btn, i, j + 3);
					}
					else
					{
						grid.Children.Add (btn, i+3, j + 1);
					}

					btn.Clicked += CellClicked;
				}
			}

		}

		private async Task<bool> CheckComplete()
		{
			bool complete = false;
			if (sudoku.IsComplete () && sudoku.IsValid ()) 
			{
				//sudoku.UpdateGameTime ();
				if (gameLog != null) 
				{
					Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
						hintBtn.SetValue(Button.IsEnabledProperty, false);
						solveBtn.SetValue(Button.IsEnabledProperty, false);
					});

					GameRecord record = new GameRecord (sudoku);
					gameLog.GameRecords.Add (record);
					timer.Stop ();

					await DisplayAlert ("Sudoku", "Completed in " + record.TimeString, "OK", "");

					await DisplayAlert ("Sudoku", gameLog.GetDisplayString (), "OK", "");
				
					gameLog.SaveGameRecords (SudokuFactory.GameLogFile);
				}

				complete = true;
			}
			return complete;
		}

		private void CreateSelectionButtons(Grid grid)
		{
			SelectionButtonList.Clear ();
			for (int i = 0; i < 9; i++) 
			{
				Button btn = new Button ();
				btn.Text = (i+1).ToString ();
				btn.HeightRequest = btnHeight;
				btn.WidthRequest = btnWidth;
				btn.Clicked += NumberPicked;

				selectionBox.HeightRequest = btnHeight / 2;
				selectionBox.WidthRequest = btnWidth;

				if (i+1 == WrongChoice) 
				{
					btn.BackgroundColor = Color.Maroon;
				}
				else
				{
					btn.BackgroundColor = Color.Default;
				}

				SelectionButtonList.Add (btn);

				if (IsVertical) 
				{
					grid.Children.Add (selectionBox, i, 13);			
					grid.Children.Add (btn, i, 15);
				}
				else
				{
					grid.Children.Add (selectionBox, 14, i+1);			
					grid.Children.Add (btn, 14, i+1);
				}
			
			}

			if (!IsVertical) 
			{
				BoxView mBox = new BoxView();
				grid.Children.Add (mBox, 13, 14, 1, 10);
			}
		}

		private void CreateClearButtons(Grid grid)
		{
			ClearAll = new Button ();
			ClearSelected = new Button ();

			ClearSelected.Clicked += NumberPicked;
			ClearSelected.HeightRequest = btnHeight;
			ClearSelected.Text = "Clear Cell";

			ClearAll.Clicked += ClearAllClick;
			ClearAll.HeightRequest = btnHeight;
			ClearAll.Text = "Clear All";


			undoBtn.Text = "Undo";
			undoBtn.HeightRequest = btnHeight;
			undoBtn.Clicked += UndoButtonClicked;
			undoBtn.IsEnabled = sudoku.UserSolvedCells.Count > 0;

			if (IsVertical) 
			{
				grid.Children.Add (undoBtn, 0, 3, 17, 18);
				grid.Children.Add (ClearSelected, 3, 6, 17, 18);
				grid.Children.Add (ClearAll, 6, 9, 17, 18);
			}
			else 
			{
				grid.Children.Add (undoBtn, 0, 1, 7, 8);
				grid.Children.Add (ClearSelected, 0, 1, 8, 9);
				grid.Children.Add (ClearAll, 0, 1, 9, 10);
			}
		}

		private void CreateTitleLabel(Grid grid)
		{
			titleLabel.TextColor = Color.White;
			titleLabel.Text = "Sudoku";
			titleLabel.HeightRequest = btnHeight;
			Font font = new Font ();
			font = Font.SystemFontOfSize (btnHeight-5, FontAttributes.Bold);

			titleLabel.Font = font;
			titleLabel.XAlign = TextAlignment.Center;
			titleLabel.YAlign = TextAlignment.Center;

			if (IsVertical) 
			{
				grid.Children.Add (titleLabel, 0, 9, 0, 1);
			}
			else
			{
				grid.Children.Add (titleLabel, 0, 1, 1, 2);
			}
			
		}

		private void CreateBoxes(Grid grid)
		{
			blackBox.Color = Color.Black;
			blackBox.HeightRequest = btnHeight * 9;
			blackBox.WidthRequest = btnWidth * 9 ;

			if (IsVertical)
			{
				grid.Children.Add (blackBox, 0, 9, 3, 12);
			}
			else
			{
				grid.Children.Add (blackBox, 3, 12, 1, 10);
			}

			BackBoxes.Clear ();
			for (int i = 0; i < 9; i += 3) 
			{
				for (int j = 0; j < 9; j += 3) 
				{ 
					BoxView box = new BoxView ();
					box.HeightRequest = btnHeight * 3 - 5;
					box.WidthRequest = btnWidth * 3 - 5;
					box.Color = Color.Gray;

					if (IsVertical) 
					{
						grid.Children.Add (box, i, i + 3, j + 3, j + 6);
					}
					else
					{
						grid.Children.Add (box, i+3, i + 6, j + 1, j + 4);
					}
					BackBoxes.Add (box);
				}
			}
		}
			

		public void CreateLayoutThread()
		{
			Thread thread = new Thread (CreateSudokuAndLayout);
			thread.Start ();
		}

		public void CreateSudokuAndLayout()
		{
			sudoku.CopyNextSudoku ();
			CreateLayout ();
		}

		public void CreateLayout()
		{
			grid.HorizontalOptions = LayoutOptions.CenterAndExpand;
			//grid.Padding = new Thickness (offsetX, offsetY, 0, 0);
			grid.ColumnSpacing = 2.0;
			grid.RowSpacing = 2.0; 

			CreateControls (grid);

			CreateBoxes (grid);

			CreateGridButtons (grid);
		
			CreateTitleLabel (grid);

			CreateSelectionButtons (grid);
				
			CreateClearButtons (grid);

			layout.Children.Add (grid);

			this.Content = layout;

		}

		private Cell GetCellByName(string btnName)
		{
			Cell cell = null;
			//Cell_0_0
			if (!string.IsNullOrEmpty (btnName))
			{
				string[] strArr = btnName.Split ('_');
				if (strArr.Length > 2) 
				{
					int i, j;
					bool success = false;
					success = Int32.TryParse (strArr [1], out i);
					success &= Int32.TryParse (strArr [2], out j);
					if (success) 
					{
						cell = sudoku.Cells [i, j];
					}
				}
			}
			return cell;
		}


		private void UpdateSelectionButtons()
		{
			if (SelectionButtonList.Count == 9) 
			{
				for(int i = 0; i < 9; i++)
				{
					Button btn = SelectionButtonList [i];
					if (sudoku.SelectedCell != null) 
					{
						//btn.IsEnabled = i == 0 || sudoku.SelectedCell.Possibles.Contains (i);
					}
					Color backgroundColor = Color.Default;

					if (i + 1 == WrongChoice) 
					{
						backgroundColor = Color.Maroon;
					}

					if (btn.BackgroundColor != backgroundColor) {
						btn.SetValue (Button.BackgroundColorProperty, backgroundColor);
					}
				}
			}
		}

		private void UpdateButtonsThread()
		{
			Thread thread = new Thread (DoUpdateButtons);
			thread.Start ();
		}

		private void DoUpdateButtons()
		{
			int i = 0;
			foreach (Button btn in ButtonList) 
			{
				if (progressBar.IsVisible) 
				{
					double percent = (double)i / 81.0;
					i++;
						Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
						progressBar.SetValue (ProgressBar.ProgressProperty, (percent));

					});
				}

				Cell cell = GetCellByName (btn.ClassId);
				bool isEnabled = true;
				Color textColor = Color.Black;
				Color backgroundColor = Color.White;
				string text = String.Empty;

				if (cell.Value != 0 && cell.ReadOnly) 
				{
					isEnabled = false;
				}

				if (cell.Highlight) 
				{
					textColor = Color.Green;
				}

				if (sudoku.SelectedCell != null && 
					cell.Value == sudoku.SelectedCell.Value) 
				{
					textColor = Color.Blue;
				}
				if (cell.ReadOnly) 
				{
					backgroundColor = Color.Silver;
				}
				else if (sudoku.SelectedCell != null && 
					sudoku.SelectedCell.Row == cell.Row && sudoku.SelectedCell.Column == cell.Column) 
				{
					backgroundColor = Color.Default;
				}

				if (cell.Value != 0) 
				{
					text = cell.Value.ToString ();
				}

				Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {

				if (btn.Text != text) 
				{
					btn.SetValue (Button.TextProperty, text);
				}
				if (btn.TextColor != textColor) 
				{
					btn.SetValue (Button.TextColorProperty, textColor);
				}
				if (btn.BackgroundColor != backgroundColor) 
				{
					btn.SetValue (Button.BackgroundColorProperty, backgroundColor);
				}

				});
			}

			if (!ClearAll.IsEnabled) 
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
					if(ClearAll != null)
					{
						ClearAll.SetValue(Button.IsEnabledProperty, true);
					}
				});
			}


			if (!newGameBtn.IsEnabled) 
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
					if(newGameBtn != null)
					{
						newGameBtn.SetValue(Button.IsEnabledProperty, true);
					}
				});
			}

			if (solveBtn.IsEnabled) 
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
					if(solveBtn != null)
					{
						solveBtn.SetValue(Button.IsEnabledProperty, true);
					}
				});
			}

			if (sudoku.UserSolvedCells.Count > 0 && !undoBtn.IsEnabled) 
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
					if(undoBtn != null)
					{
						undoBtn.SetValue (Button.IsEnabledProperty, true);
					}
				});
			}
			else if(sudoku.UserSolvedCells.Count == 0 && undoBtn.IsEnabled) 
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
					if(undoBtn != null)
					{
						undoBtn.SetValue(Button.IsEnabledProperty, false);
					}
				});
			}

			if (progressBar.IsVisible) 
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
					progressBar.SetValue(ProgressBar.ProgressProperty, 0.0);
					progressBar.SetValue(ProgressBar.IsVisibleProperty, false);
				});
			}


			if (sudoku != null && timer != null && sudoku.Creating) 
			{
				//sudoku.gameTime = new TimeSpan ();
				//sudoku.startTime = DateTime.Now;

				sudoku.TimeCount = 0;
				timer.Start ();
				sudoku.Creating = false; 
			}

		}

		private void UpdateButtons()
		{
			//CreateLayout ();
			UpdateButtonsThread ();
			//DoUpdateButtons ();
		}
			

		#region Event Handlers

		private async void ClearAllClick(object sender, EventArgs e)
		{
			timer.Stop ();
			var answer = await DisplayAlert ("Sudoku", "Clear All Cells?", "Yes", "No");
			if (answer) {
				Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
					ClearAll.SetValue (Button.IsEnabledProperty, false);
					progressBar.SetValue(ProgressBar.IsVisibleProperty, true);
				});

				sudoku.Clear ();
				UpdateButtons ();
				UpdateSelectionButtons ();
			}
			else 
			{
				timer.Start ();
			}
		}

		private void NumberPicked(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			int num = 0;
			if ((btn.Text == "Clear Cell" || Int32.TryParse (btn.Text, out num))
				&& sudoku.SelectedCell != null)  
			{
				if ((btn.Text == "Clear Cell" || sudoku.Cells [sudoku.SelectedCell.Row, sudoku.SelectedCell.Column].Possibles.Contains (num)) 
					&& !sudoku.Cells[sudoku.SelectedCell.Row, sudoku.SelectedCell.Column].ReadOnly)
				{
					//sudoku.Cells [sudoku.SelectedCell.Row, sudoku.SelectedCell.Column].Value = num;
					sudoku.SelectedCell.Value = num;
					sudoku.UpdateCellPossiblesThread ();

					if (!sudoku.SelectedCell.ReadOnly) 
					{
						sudoku.UserSolvedCells.Add (sudoku.SelectedCell);
					}

					WrongChoice = -1;
					UpdateButtons ();
					CheckComplete ();
				}
				else
				{
					if(!Int32.TryParse(btn.Text, out WrongChoice))
					{
						WrongChoice = -1;
					}

				}
				UpdateSelectionButtons ();

			}
		}

		private void HintButtonClick(object sender, EventArgs e)
		{
			sudoku.RevealOneCell ();
			CheckComplete ();
			UpdateButtons ();
		}

		private void SolveButtonClick(object sender, EventArgs e)
		{
			Xamarin.Forms.Device.BeginInvokeOnMainThread (() => 
				{
					solveBtn.SetValue(Button.IsEnabledProperty, false);
					progressBar.SetValue(ProgressBar.IsVisibleProperty, true);
				});


			if (!sudoku.RevealAllCells ()) 
			{
				sudoku.solution = null;
				while (!sudoku.IsComplete ()) 
				{
					while (sudoku.SolveOneCell ()) 
					{

					}
					if (!sudoku.IsComplete ())
					{
						Sudoku tmp = new Sudoku (sudoku);
						if (!sudoku.CreateRandom ()) 
						{
							sudoku = new Sudoku (tmp);
						}
					}
				}
			}

			CheckComplete ();

			UpdateButtons ();

		}

		private void NewGameButtonClick(object sender, EventArgs e)
		{
			Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
				newGameBtn.SetValue(Button.IsEnabledProperty, false);

				progressBar.SetValue(ProgressBar.IsVisibleProperty, true);

				if(!hintBtn.IsEnabled)
				{
					hintBtn.SetValue(Button.IsEnabledProperty, true);
				}
				if(!solveBtn.IsEnabled)
				{
					solveBtn.SetValue(Button.IsEnabledProperty, true);
				}
			});

			timer.Stop ();

			sudoku.NewGame ();
			//DoUpdateButtons ();

			UpdateSelectionButtons ();
			UpdateButtons ();

			//NextGameGrid.CreateLayoutThread ();

		}


		private void DifficultyIndexChange(object sender, EventArgs e)
		{
			Picker picker = (Picker)sender;
			string item = picker.Items [picker.SelectedIndex].ToString ();
			Difficulty diff;
			if (Enum.TryParse (item, out diff)) 
			{
				sudoku.difficulty = diff;
			}

		}

		private void CellClicked(object sender, EventArgs e)
		{
			Button btn = (Button)sender;

			sudoku.SelectedCell = GetCellByName(btn.ClassId);
			sudoku.UpdateCellHighlights ();
			WrongChoice = -1;
			UpdateButtons ();
			UpdateSelectionButtons ();
		}


		private void UndoButtonClicked(object sender, EventArgs e)
		{
			sudoku.UndoCell ();
			UpdateButtons ();
		}

		private void TimerTicked(object sender, EventArgs e)
		{
			if (!newGameBtn.IsEnabled) 
			{
				sudoku.TimeCount = 0;
			}
			if (!sudoku.Creating) 
			{
				sudoku.TimeCount++;
				Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
					timerLabel.SetValue (Label.TextProperty, sudoku.TimeString);
				});
			}
		}


		#endregion Event Handlers
	}
}

