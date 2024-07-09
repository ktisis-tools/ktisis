﻿using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.Havok.Animation.Rig;

namespace Ktisis.Scene.Decor;

public interface ISkeleton {
	public unsafe Skeleton* GetSkeleton();
	public unsafe hkaPose* GetPose(int index);
}
