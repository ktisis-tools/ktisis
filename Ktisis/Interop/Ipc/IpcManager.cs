﻿using System.Linq;

using Dalamud.Plugin;

using Ktisis.Core.Attributes;

namespace Ktisis.Interop.Ipc;

[Singleton]
public class IpcManager {
	private readonly IDalamudPluginInterface _dpi;

	public IpcManager(
		IDalamudPluginInterface dpi
	) {
		this._dpi = dpi;
	}

	public bool IsPenumbraActive => this.GetPluginInstalled("Penumbra");
	
	public PenumbraIpcProvider GetPenumbraIpc() => new(this._dpi);

	private bool GetPluginInstalled(string internalName)
		=> this._dpi.InstalledPlugins.Any(p => p.IsLoaded && p.InternalName == internalName);
}
