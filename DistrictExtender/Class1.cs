using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;

using Timberborn.Navigation;
using Timberborn.BuildingsNavigation;

namespace DistrictExtender
{
    [BepInPlugin("DistrictExtender.Procdox.com.github", "DistrictExtender", "1.0.0.0")]
    [HarmonyPatch]
    public class Patcher : BaseUnityPlugin
    {
        private static BepInEx.Logging.ManualLogSource Logger;
        private static int District = 70;
        private static int Pathfinding = 55;
        private static int Resource = 20;
        private static int Terrain = 10;

        /*[HarmonyPrefix]
        [HarmonyPatch(typeof(NavigationService), "FindPathUncachedLimitedRange")]
        static bool FPULROverride(NavigationService __instance, bool __result, UnityEngine.Vector3 start, UnityEngine.Vector3 end, out float distance, List<UnityEngine.Vector3> pathCorners)
        {
            if (__instance.DestinationIsReachableUnlimitedRange(start, end, out distance, pathCorners))
            {
                __result = __instance._pathfindingService.FindPathUncached(start, end, 300f, out distance, pathCorners);
            }
            else
            {
                __result = false;
            }
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DistrictMap), "RecalculateRoadFlowField")]
        static bool RRFFOverride(DistrictMap __instance, AccessFlowField roadFlowField, int districtCenterNodeId)
        {
            __instance._districtRoadFlowFieldGenerator.FillFlowFieldUpToDistance(__instance._roadNavMeshGraph, __instance._districtObstacleService, roadFlowField, 300f, districtCenterNodeId);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PathMeshDrawer), "DistanceToColor")]
        static bool DTCOverride(PathMeshDrawer __instance, UnityEngine.Color __result, float distance)
        {
            float time = UnityEngine.Mathf.InverseLerp(0f, 300f, distance);
            __result = __instance._distanceGradient.Evaluate(time);
            return false;
        }*/

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NavigationDistance), "Load")]
        static bool DTCOverride(NavigationDistance __instance)
        {
            FieldInfo DistrictRoadField = typeof(NavigationDistance).GetField("<DistrictRoad>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            DistrictRoadField.SetValue(__instance, District);
            FieldInfo LimitedField = typeof(NavigationDistance).GetField("<Limited>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            LimitedField.SetValue(__instance, Pathfinding);
            FieldInfo ResourceBuildingsField = typeof(NavigationDistance).GetField("<ResourceBuildings>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            ResourceBuildingsField.SetValue(__instance, Resource);
            FieldInfo DistrictTerrainField = typeof(NavigationDistance).GetField("<DistrictTerrain>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            DistrictTerrainField.SetValue(__instance, Terrain);
            return false;
        }
        void Awake()
        {
            Logger = base.Logger;
            District = Config.Bind<int>("CONFIG", "District", 70, "The max distance of a district's influence.").Value;
            Pathfinding = Config.Bind<int>("CONFIG", "Pathfinding", 55, "The max distance of pathfinding on roads.").Value;
            Resource = Config.Bind<int>("CONFIG", "Resource", 20, "The max distance of resource gathering over terrain.").Value;
            Terrain = Config.Bind<int>("CONFIG", "Terrain", 10, "The max distance of pathfinding on terrain.").Value;
            var harmony = new Harmony("DistrictExtender.Procdox.com.github");
            harmony.PatchAll();
        }
    }
}
