using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using Button = Xamarin.Forms.Button;
using Android;
using Android.Content.PM;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform;

namespace Sudoku
{
	[Activity (Label = "Sudoku", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		public static Activity ActivityContext;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			ActivityContext = this;

			int width = ConvertPixelsToDp(Resources.DisplayMetrics.WidthPixels);
			int height = ConvertPixelsToDp (Resources.DisplayMetrics.HeightPixels);

			Xamarin.Forms.Forms.Init (this, bundle);

			SudokuFactory.Width = width;
			SudokuFactory.Height = height;

            LoadApplication(new App());

		}

		private int ConvertPixelsToDp(float pixels)
		{
			return (int)((pixels) / Resources.DisplayMetrics.Density);
		}

		protected override void OnStop()
		{
			base.OnStop ();

			SudokuFactory.Sudoku.SaveToFile (SudokuFactory.SavedSudokuFile);

			SudokuFactory.Sudoku.SaveSolution (SudokuFactory.SavedSolutionFile);

			SudokuFactory.Sudoku.SaveStartTime (SudokuFactory.SavedGameTimeFile);

		}


		protected override void OnStart()
		{
			base.OnStart ();

			SudokuFactory.Sudoku.LoadFromFile (SudokuFactory.SavedSudokuFile);

			SudokuFactory.Sudoku.LoadSolution(SudokuFactory.SavedSolutionFile);

			SudokuFactory.Sudoku.LoadStartTime (SudokuFactory.SavedGameTimeFile);

		}

	}
}


