﻿/* 
QuickExit
Copyright 2017 Malah

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. 
*/

using KSP.UI.Screens;
using UnityEngine;

namespace QuickExit {
	public partial class QStockToolbar {

		internal static bool Enabled {
			get {
				return QSettings.Instance.StockToolBar;
			}
		}

		static bool ModApp {
			get {
				return QSettings.Instance.StockToolBar_ModApp;
			}
		}

		static bool CanUseIt {
			get {
				return HighLogic.LoadedSceneIsGame;
			}
		}

		ApplicationLauncher.AppScenes AppScenes = ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.VAB;
		static string TexturePath = relativePath + "/Textures/StockToolBar";

		void OnClick() { 
			QExit.Instance.Dialog ();
			Log ("OnClick", "QStockToolbar");
		}
			
		Texture2D GetTexture {
			get {
				return GameDatabase.Instance.GetTexture(TexturePath, false);
			}
		}

		ApplicationLauncherButton appLauncherButton;

		internal static bool isAvailable {
			get {
				return ApplicationLauncher.Ready && ApplicationLauncher.Instance != null;
			}
		}

		internal static bool isModApp(ApplicationLauncherButton button) {
			bool _hidden;
			return ApplicationLauncher.Instance.Contains (button, out _hidden);
		}

		internal static QStockToolbar Instance {
			get;
			private set;
		}

		protected override void Awake() {
			if (Instance != null) {
				Destroy (this);
				return;
			}
			Instance = this;
			DontDestroyOnLoad (this);
			GameEvents.onGUIApplicationLauncherReady.Add (AppLauncherReady);
			GameEvents.onGUIApplicationLauncherDestroyed.Add (AppLauncherDestroyed);
			GameEvents.onLevelWasLoadedGUIReady.Add (AppLauncherDestroyed);
			Log ("Awake", "QStockToolbar");
		}

		protected override void Start() {
			Log ("Start", "QStockToolbar");
		}
			
		void AppLauncherReady() {
			if (!Enabled) {
				return;
			}
			Init ();
			Log ("AppLauncherReady", "QStockToolbar");
		}

		void AppLauncherDestroyed(GameScenes gameScene) {
			if (CanUseIt) {
				return;
			}
			Destroy ();
			Log ("AppLauncherDestroyed", "QStockToolbar");
		}

		void AppLauncherDestroyed() {
			Destroy ();
			Log ("AppLauncherDestroyed", "QStockToolbar");
		}

		protected override void OnDestroy() {
			GameEvents.onGUIApplicationLauncherReady.Remove (AppLauncherReady);
			GameEvents.onGUIApplicationLauncherDestroyed.Remove (AppLauncherDestroyed);
			GameEvents.onLevelWasLoadedGUIReady.Remove (AppLauncherDestroyed);
			Log ("OnDestroy", "QStockToolbar");
		}

		void Init() {
			if (!isAvailable || !CanUseIt) {
				return;
			}
			if (appLauncherButton == null) {
				if (ModApp) {
					appLauncherButton = ApplicationLauncher.Instance.AddModApplication (OnClick, null, null, null, null, null, AppScenes, GetTexture);
				} else {
					appLauncherButton = ApplicationLauncher.Instance.AddApplication (OnClick, null, null, null, null, null, GetTexture);
					appLauncherButton.VisibleInScenes = ApplicationLauncher.AppScenes.ALWAYS;
					ApplicationLauncher.Instance.DisableMutuallyExclusive (appLauncherButton);
				}
			}
			Log ("Init", "QStockToolbar");
		}

		void Destroy() {
			if (appLauncherButton != null) {
				ApplicationLauncher.Instance.RemoveModApplication (appLauncherButton);
				ApplicationLauncher.Instance.RemoveApplication (appLauncherButton);
				appLauncherButton = null;
			}
			Log ("Destroy", "QStockToolbar");
		}

		internal void Set(bool SetTrue, bool force = false) {
			if (!isAvailable) {
				return;
			}
			if (appLauncherButton != null) {
				if (SetTrue) {
					if (appLauncherButton.toggleButton.CurrentState == KSP.UI.UIRadioButton.State.False) {
						appLauncherButton.SetTrue (force);
					}
				} else {
					if (appLauncherButton.toggleButton.CurrentState == KSP.UI.UIRadioButton.State.True) {
						appLauncherButton.SetFalse (force);
					}
				}
			}
			Log ("Set " + SetTrue + " force: " + force, "QStockToolbar");
		}

		internal void Reset() {
			if (appLauncherButton != null) {
				Set (false);
				if (!Enabled || (Enabled && (ModApp && !isModApp(appLauncherButton)) || (!ModApp && isModApp(appLauncherButton)))) {
					Destroy ();
				}
			}
			if (Enabled) {
				Init ();
			}
			Log ("Reset", "QStockToolbar");
		}
	}
}