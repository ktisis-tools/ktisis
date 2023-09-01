using System.Linq;
using System.Numerics;
using System.Collections.Generic;

using Ktisis.Core;
using Ktisis.Scene;
using Ktisis.Scene.Impl;
using Ktisis.Scene.Objects;
using Ktisis.Interface.Helpers;

namespace Ktisis.Interface.Overlay;

public class SceneOverlay {
	// Constructor

	private readonly SceneManager _scene;

	private readonly DotSelection DotSelection;

	public SceneOverlay(IServiceContainer _services, SceneManager _scene) {
		this._scene = _scene;

		this.DotSelection = _services.Inject<DotSelection>();
		this.DotSelection.OnItemSelected += OnItemSelected;
	}

	// Events

	public void SubscribeTo(GuiOverlay overlay) {
		overlay.OnOverlayDraw += OnOverlayDraw;
		if (overlay.Gizmo is Gizmo gizmo)
			gizmo.OnManipulate += OnManipulate;
	}

	private void OnItemSelected(SceneObject item) {
		var flags = GuiSelect.GetSelectFlags();
		this._scene.SelectState?.HandleClick(item, flags);
	}

	// Draw UI

	private void OnOverlayDraw(GuiOverlay _overlay) {
		var scene = this._scene.Scene;
		if (scene is null) return;

		this.DotSelection.Clear();
		DrawItems(_overlay, scene.GetChildren());
		this.DotSelection.DrawHoverWindow();
		DrawSelected(_overlay);
	}

	private void DrawItems(GuiOverlay _overlay, IEnumerable<SceneObject> items) {
		foreach (var item in items)
			DrawItem(_overlay, item);
	}

	private void DrawItem(GuiOverlay _overlay, SceneObject item) {
		if (item is IManipulable)
			this.DotSelection.AddItem(item);
		DrawItems(_overlay, item.GetChildren());
	}

	// Gizmo

	// TODO: Config
	private TransformFlags Flags = TransformFlags.Propagate;

	private IEnumerable<IManipulable>? GetSelected() => this._scene
		.SelectState?
		.GetSelected()
		.Where(item => item is IManipulable)
		.Cast<IManipulable>();

	private void DrawSelected(GuiOverlay _overlay) {
		var selected = GetSelected();
		if (selected is null) return;

		foreach (var item in selected) {
			var matrix = item.ComposeMatrix();
			if (matrix is null) continue;

			_overlay.Gizmo?.Manipulate(matrix.Value);

			break;
		}
	}

	private void OnManipulate(Gizmo gizmo) {
		var selected = GetSelected();
		if (selected is null) return;

		var result = gizmo.GetResult();
		Matrix4x4? delta = null;

		var isPrimary = true;
		foreach (var item in selected) {
			var matrix = item.ComposeMatrix();
			if (matrix is null) continue;

			if (isPrimary) {
				item.SetMatrix(result, this.Flags);
				isPrimary = false;
			} else {
				item.SetMatrix(gizmo.ApplyDelta(
					matrix.Value,
					delta ??= gizmo.GetDelta(),
					result
				), this.Flags);
			}
		}
	}
}
