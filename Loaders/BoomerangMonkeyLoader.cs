using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.Api;
using Il2Cpp;

namespace UltimateCrosspathing.Loaders;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Runtime.Serialization;
using Il2CppSystem.Reflection;
using Il2CppSystem;
using Il2CppAssets.Scripts.Simulation.SMath;
using System.IO;

public class BoomerangMonkeyLoader : ModByteLoader<Il2CppAssets.Scripts.Models.Towers.TowerModel> {
	
	BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static; 
	BinaryReader br = null;
	
	// NOTE: was a collection per type but it prevented inheriance e.g list of Products would required class type id
	protected override string BytesFileName => "BoomerangMonkeys.bytes";
	int mIndex = 1; // first element is null
	#region Read array
	
	private void LinkArray<T>() where T : Il2CppObjectBase {
		var setCount = br.ReadInt32();
		for (var i = 0; i < setCount; i++) {
			var arrIndex = br.ReadInt32();
			var arr = (Il2CppReferenceArray<T>)m[arrIndex];
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = (T) m[br.ReadInt32()];
			}
		}
	}
	private void LinkList<T>() where T : Il2CppObjectBase {
		var setCount = br.ReadInt32();
		for (var i = 0; i < setCount; i++) {
			var arrIndex = br.ReadInt32();
			var arr = (List<T>)m[arrIndex];
			for (var j = 0; j < arr.Capacity; j++) {
				arr.Add( (T) m[br.ReadInt32()] );
			}
		}
	}
	private void LinkDictionary<T>() where T : Il2CppObjectBase {
		var setCount = br.ReadInt32();
		for (var i = 0; i < setCount; i++) {
			var arrIndex = br.ReadInt32();
			var arr = (Dictionary<string, T>)m[arrIndex];
			var arrCount = br.ReadInt32();
			for (var j = 0; j < arrCount; j++) {
				var key = br.ReadString();
				var valueIndex = br.ReadInt32();
				arr[key] = (T) m[valueIndex];
			}
		}
	}
	private void LinkModelDictionary<T>() where T : Il2CppAssets.Scripts.Models.Model {
		var setCount = br.ReadInt32();
		for (var i = 0; i < setCount; i++) {
			var arrIndex = br.ReadInt32();
			var arr = (Dictionary<string, T>)m[arrIndex];
			var arrCount = br.ReadInt32();
			for (var j = 0; j < arrCount; j++) {
				var valueIndex = br.ReadInt32();
				var obj = (T)m[valueIndex];
				arr[obj.name] = obj;
			}
		}
	}
	private void Read_a_Int32_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppStructArray<int>(arrCount);
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = br.ReadInt32();
			}
			m[mIndex++] = arr;
		}
	}
	private void Read_a_Single_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppStructArray<float>(arrCount);
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = br.ReadSingle();
			}
			m[mIndex++] = arr;
		}
	}
	private void Read_a_String_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppStringArray(arrCount);
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = br.ReadBoolean() ? null : br.ReadString();
			}
			m[mIndex++] = arr;
		}
	}
	private void Read_a_Vector3_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppStructArray<Il2CppAssets.Scripts.Simulation.SMath.Vector3>(arrCount);
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
			}
			m[mIndex++] = arr;
		}
	}
	private void Read_a_TargetType_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.TargetType>(arrCount);
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = new Il2CppAssets.Scripts.Models.Towers.TargetType {id = br.ReadString(), isActionable = br.ReadBoolean()};
			}
			m[mIndex++] = arr;
		}
	}
	private void Read_a_AreaType_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppAssets.Scripts.Models.Map.AreaType[arrCount];
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = (Il2CppAssets.Scripts.Models.Map.AreaType)br.ReadInt32();
			}
			m[mIndex++] = arr;
		}
	}
	#endregion
	
	#region Read object records
	
	private void CreateArraySet<T>() where T : Il2CppObjectBase {
		var arrCount = br.ReadInt32();
		for(var i = 0; i < arrCount; i++) {
			m[mIndex++] = new Il2CppReferenceArray<T>(br.ReadInt32());;
		}
	}
	
	private void CreateListSet<T>() where T : Il2CppObjectBase {
		var arrCount = br.ReadInt32();
		for (var i = 0; i < arrCount; i++) {
			m[mIndex++] = new List<T>(br.ReadInt32()); // set capactity
		}
	}
	
	private void CreateDictionarySet<K, T>() {
		var arrCount = br.ReadInt32();
		for (var i = 0; i < arrCount; i++) {
			m[mIndex++] = new Dictionary<K, T>(br.ReadInt32());// set capactity
		}
	}
	
	private void Create_Records<T>() where T : Il2CppObjectBase {
		var count = br.ReadInt32();
		var t = Il2CppType.Of<T>();
		for (var i = 0; i < count; i++) {
			m[mIndex++] = FormatterServices.GetUninitializedObject(t).Cast<T>();
		}
	}
	#endregion
	
	#region Link object records
	
	private void Set_v_Model_Fields(int start, int count) {
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Model>();
		var _nameField = t.GetField("_name", bindFlags);
		var childDependantsField = t.GetField("childDependants", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Model)m[i+start];
			_nameField.SetValue(v,br.ReadBoolean() ? null : String.Intern(br.ReadString()));
			childDependantsField.SetValue(v,(List<Il2CppAssets.Scripts.Models.Model>) m[br.ReadInt32()]);
		}
	}
	
	private void Set_v_TowerModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.TowerModel)m[i+start];
			v.display = ModContent.CreatePrefabReference(br.ReadString());
			v.baseId = br.ReadBoolean() ? null : br.ReadString();
			v.cost = br.ReadSingle();
			v.radius = br.ReadSingle();
			v.radiusSquared = br.ReadSingle();
			v.range = br.ReadSingle();
			v.ignoreBlockers = br.ReadBoolean();
			v.isGlobalRange = br.ReadBoolean();
			v.tier = br.ReadInt32();
			v.tiers = (Il2CppStructArray<int>) m[br.ReadInt32()];
			v.towerSet = (Il2CppAssets.Scripts.Models.TowerSets.TowerSet) (br.ReadInt32());
			var whoCares = m[br.ReadInt32()];
			v.areaTypes = null;
			v.icon = ModContent.CreateSpriteReference(br.ReadString());
			v.portrait = ModContent.CreateSpriteReference(br.ReadString());
			v.instaIcon = ModContent.CreateSpriteReference(br.ReadString());
			v.mods = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Mods.ApplyModModel>) m[br.ReadInt32()];
			v.ignoreTowerForSelection = br.ReadBoolean();
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Model>) m[br.ReadInt32()];
			v.footprint = (Il2CppAssets.Scripts.Models.Towers.Behaviors.FootprintModel) m[br.ReadInt32()];
			v.dontDisplayUpgrades = br.ReadBoolean();
			v.emoteSpriteSmall = ModContent.CreateSpriteReference(br.ReadString());
			v.emoteSpriteLarge = ModContent.CreateSpriteReference(br.ReadString());
			v.doesntRotate = br.ReadBoolean();
			v.upgrades = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel>) m[br.ReadInt32()];
			v.appliedUpgrades = (Il2CppStringArray) m[br.ReadInt32()];
			v.targetTypes = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.TargetType>) m[br.ReadInt32()];
			v.paragonUpgrade = (Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel) m[br.ReadInt32()];
			v.isSubTower = br.ReadBoolean();
			v.isBakable = br.ReadBoolean();
			v.powerName = br.ReadBoolean() ? null : br.ReadString();
			v.showPowerTowerBuffs = br.ReadBoolean();
			v.animationSpeed = br.ReadSingle();
			v.towerSelectionMenuThemeId = br.ReadBoolean() ? null : br.ReadString();
			v.ignoreCoopAreas = br.ReadBoolean();
			v.canAlwaysBeSold = br.ReadBoolean();
			v.blockSelling = br.ReadBoolean();
			v.isParagon = br.ReadBoolean();
			v.ignoreMaxSellPercent = br.ReadBoolean();
			v.isStunned = br.ReadBoolean();
			v.geraldoItemName = br.ReadBoolean() ? null : br.ReadString();
			v.sellbackModifierAdd = br.ReadSingle();
			v.skinName = br.ReadBoolean() ? null : br.ReadString();
			v.dontAddMutatorsFromParent = br.ReadBoolean();
			v.displayScale = br.ReadSingle();
			v.showBuffs = br.ReadBoolean();
		}
	}
	
	private void Set_v_ApplyModModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Mods.ApplyModModel)m[i+start];
			v.mod = br.ReadBoolean() ? null : br.ReadString();
			v.target = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_TowerBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.TowerBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_CreateSoundOnSellModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnSellModel)m[i+start];
			v.sound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_SoundModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Audio.SoundModel)m[i+start];
			v.assetId = ModContent.CreateAudioSourceReference(br.ReadString());
		}
	}
	
	private void Set_v_CreateSoundOnUpgradeModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnUpgradeModel)m[i+start];
			v.sound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound1 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound3 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound4 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound5 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound6 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound7 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound8 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateEffectOnSellModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnSellModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_EffectModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Effects.EffectModel)m[i+start];
			v.assetId = ModContent.CreatePrefabReference(br.ReadString());
			v.scale = br.ReadSingle();
			v.lifespan = br.ReadSingle();
			v.fullscreen = br.ReadBoolean();
			v.useCenterPosition = br.ReadBoolean();
			v.useTransformPosition = br.ReadBoolean();
			v.useTransfromRotation = br.ReadBoolean();
			v.destroyOnTransformDestroy = br.ReadBoolean();
			v.alwaysUseAge = br.ReadBoolean();
			v.useRoundTime = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateEffectOnPlaceModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnPlaceModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateSoundOnTowerPlaceModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnTowerPlaceModel)m[i+start];
			v.sound1 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound1 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateEffectOnUpgradeModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnUpgradeModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.createOnAirUnit = br.ReadBoolean();
		}
	}
	
	private void Set_v_AttackModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackModel)m[i+start];
			v.weapons = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel>) m[br.ReadInt32()];
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Model>) m[br.ReadInt32()];
			v.range = br.ReadSingle();
			v.targetProvider = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSupplierModel) m[br.ReadInt32()];
			v.offsetX = br.ReadSingle();
			v.offsetY = br.ReadSingle();
			v.offsetZ = br.ReadSingle();
			v.attackThroughWalls = br.ReadBoolean();
			v.fireWithoutTarget = br.ReadBoolean();
			v.framesBeforeRetarget = br.ReadInt32();
			v.addsToSharedGrid = br.ReadBoolean();
			v.sharedGridRange = br.ReadSingle();
		}
	}
	
	private void Set_v_WeaponModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel>();
		var animationOffsetField = t.GetField("animationOffset", bindFlags);
		var rateField = t.GetField("rate", bindFlags);
		var customStartCooldownField = t.GetField("customStartCooldown", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel)m[i+start];
			v.animation = br.ReadInt32();
			animationOffsetField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.emission = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel) m[br.ReadInt32()];
			v.ejectX = br.ReadSingle();
			v.ejectY = br.ReadSingle();
			v.ejectZ = br.ReadSingle();
			v.projectile = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.fireWithoutTarget = br.ReadBoolean();
			v.fireBetweenRounds = br.ReadBoolean();
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>) m[br.ReadInt32()];
			rateField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.useAttackPosition = br.ReadBoolean();
			v.startInCooldown = br.ReadBoolean();
			customStartCooldownField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.animateOnMainAttack = br.ReadBoolean();
			v.isStunned = br.ReadBoolean();
		}
	}
	
	private void Set_v_EmissionModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[i+start];
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_SingleEmissionModel_Fields(int start, int count) {
		Set_v_EmissionModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmissionModel)m[i+start];
		}
	}
	
	private void Set_v_ProjectileModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[i+start];
			v.display = ModContent.CreatePrefabReference(br.ReadString());
			v.id = br.ReadBoolean() ? null : br.ReadString();
			v.maxPierce = br.ReadSingle();
			v.pierce = br.ReadSingle();
			v.scale = br.ReadSingle();
			v.ignoreBlockers = br.ReadBoolean();
			v.usePointCollisionWithBloons = br.ReadBoolean();
			v.canCollisionBeBlockedByMapLos = br.ReadBoolean();
			v.filters = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Model>) m[br.ReadInt32()];
			v.collisionPasses = (Il2CppStructArray<int>) m[br.ReadInt32()];
			v.canCollideWithBloons = br.ReadBoolean();
			v.radius = br.ReadSingle();
			v.vsBlockerRadius = br.ReadSingle();
			v.hasDamageModifiers = br.ReadBoolean();
			v.dontUseCollisionChecker = br.ReadBoolean();
			v.checkCollisionFrames = br.ReadInt32();
			v.ignoreNonTargetable = br.ReadBoolean();
			v.ignorePierceExhaustion = br.ReadBoolean();
			v.saveId = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_FilterModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel)m[i+start];
		}
	}
	
	private void Set_v_FilterInvisibleModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterInvisibleModel)m[i+start];
			v.isActive = br.ReadBoolean();
			v.ignoreBroadPhase = br.ReadBoolean();
		}
	}
	
	private void Set_v_ProjectileBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileBehaviorModel)m[i+start];
			v.collisionPass = br.ReadInt32();
		}
	}
	
	private void Set_v_DamageModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel)m[i+start];
			v.damage = br.ReadSingle();
			v.maxDamage = br.ReadSingle();
			v.distributeToChildren = br.ReadBoolean();
			v.overrideDistributeBlocker = br.ReadBoolean();
			v.createPopEffect = br.ReadBoolean();
			v.immuneBloonProperties = (BloonProperties) (br.ReadInt32());
			v.immuneBloonPropertiesOriginal = (BloonProperties) (br.ReadInt32());
		}
	}
	
	private void Set_v_FollowPathModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.FollowPathModel>();
		var speedField = t.GetField("speed", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.FollowPathModel)m[i+start];
			v.path = (Il2CppStructArray<Il2CppAssets.Scripts.Simulation.SMath.Vector3>) m[br.ReadInt32()];
			v.easePath = (Il2CppStructArray<Il2CppAssets.Scripts.Simulation.SMath.Vector3>) m[br.ReadInt32()];
			v.destroyAtEndOfPath = br.ReadBoolean();
			speedField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_ProjectileFilterModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileFilterModel)m[i+start];
			v.filters = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_RotateModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.RotateModel>();
		var angleField = t.GetField("angle", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.RotateModel)m[i+start];
			angleField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_DamageModifierModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.DamageModifierModel)m[i+start];
		}
	}
	
	private void Set_v_DamageModifierForTagModel_Fields(int start, int count) {
		Set_v_DamageModifierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForTagModel)m[i+start];
			v.tag = br.ReadBoolean() ? null : br.ReadString();
			v.tags = (Il2CppStringArray) m[br.ReadInt32()];
			v.damageMultiplier = br.ReadSingle();
			v.damageAddative = br.ReadSingle();
			v.mustIncludeAllTags = br.ReadBoolean();
			v.applyOverMaxDamage = br.ReadBoolean();
		}
	}
	
	private void Set_v_DisplayModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.GenericBehaviors.DisplayModel)m[i+start];
			v.display = ModContent.CreatePrefabReference(br.ReadString());
			v.layer = br.ReadInt32();
			v.positionOffset = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
			v.scale = br.ReadSingle();
			v.ignoreRotation = br.ReadBoolean();
			v.animationChanges = (List<Il2CppAssets.Scripts.Models.GenericBehaviors.AnimationChange>) m[br.ReadInt32()];
			v.delayedReveal = br.ReadSingle();
		}
	}
	
	private void Set_v_ClearHitBloonsModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.ClearHitBloonsModel>();
		var intervalField = t.GetField("interval", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.ClearHitBloonsModel)m[i+start];
			intervalField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_WeaponBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_FlipFollowPathModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.FlipFollowPathModel)m[i+start];
			v.flipTowerDisplayX = br.ReadBoolean();
			v.flipTowerDisplayY = br.ReadBoolean();
			v.effectOnFlip = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_AttackBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_RotateToTargetModel_Fields(int start, int count) {
		Set_v_AttackBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetModel)m[i+start];
			v.onlyRotateDuringThrow = br.ReadBoolean();
			v.useThrowMarkerHeight = br.ReadBoolean();
			v.rotateOnlyOnThrow = br.ReadBoolean();
			v.additionalRotation = br.ReadInt32();
			v.rotateTower = br.ReadBoolean();
			v.useMainAttackRotation = br.ReadBoolean();
		}
	}
	
	private void Set_v_AttackFilterModel_Fields(int start, int count) {
		Set_v_AttackBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.AttackFilterModel)m[i+start];
			v.filters = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_TargetSupplierModel_Fields(int start, int count) {
		Set_v_AttackBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSupplierModel)m[i+start];
			v.isOnSubTower = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetFirstModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetLastModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetCloseModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCloseModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetStrongModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_FootprintModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.FootprintModel)m[i+start];
			v.doesntBlockTowerPlacement = br.ReadBoolean();
			v.ignoresPlacementCheck = br.ReadBoolean();
			v.ignoresTowerOverlap = br.ReadBoolean();
		}
	}
	
	private void Set_v_CircleFootprintModel_Fields(int start, int count) {
		Set_v_FootprintModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CircleFootprintModel)m[i+start];
			v.radius = br.ReadSingle();
		}
	}
	
	private void Set_v_UpgradePathModel_Fields(int start, int count) {
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
		var towerField = t.GetField("tower", bindFlags);
		var upgradeField = t.GetField("upgrade", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel)m[i+start];
			towerField.SetValue(v,br.ReadBoolean() ? null : br.ReadString());
			upgradeField.SetValue(v,br.ReadBoolean() ? null : br.ReadString());
		}
	}
	
	private void Set_v_FilterOutBloonModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterOutBloonModel)m[i+start];
			v.bloonId = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_PushBackModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.PushBackModel)m[i+start];
			v.pushAmount = br.ReadSingle();
			v.tag = br.ReadBoolean() ? null : br.ReadString();
			v.multiplierBFB = br.ReadSingle();
			v.multiplierDDT = br.ReadSingle();
			v.multiplierZOMG = br.ReadSingle();
		}
	}
	
	private void Set_v_ArriveAtTargetModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.ArriveAtTargetModel)m[i+start];
			v.timeToTake = br.ReadSingle();
			v.curveSamples = (Il2CppStructArray<float>) m[br.ReadInt32()];
			v.filterCollisionWhileMoving = br.ReadBoolean();
			v.expireOnArrival = br.ReadBoolean();
			v.altSpeed = br.ReadSingle();
			v.stopOnTargetReached = br.ReadBoolean();
			v.curve = (Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors.Curve) m[br.ReadInt32()];
			v.keepUpdatingTargetPos = br.ReadBoolean();
			v.rotateToTarget = br.ReadBoolean();
			v.maxTurnAngle = br.ReadSingle();
		}
	}
	
	private void Set_v_Curve_Fields(int start, int count) {
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors.Curve>();
		var samplesField = t.GetField("samples", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors.Curve)m[i+start];
			v.samples = (Il2CppStructArray<float>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_FilterWithTagModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterWithTagModel)m[i+start];
			v.moabTag = br.ReadBoolean();
			v.camoTag = br.ReadBoolean();
			v.growTag = br.ReadBoolean();
			v.fortifiedTag = br.ReadBoolean();
			v.tag = br.ReadBoolean() ? null : br.ReadString();
			v.inclusive = br.ReadBoolean();
			v.hasMoabTag = br.ReadBoolean();
		}
	}
	
	private void Set_v_FilterOutTagModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterOutTagModel)m[i+start];
			v.tag = br.ReadBoolean() ? null : br.ReadString();
			v.disableWhenSupportMutatorIDs = (Il2CppStringArray) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_SlowModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorWithOverlayModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.SlowModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		var multiplierField = t.GetField("multiplier", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.SlowModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.layers = br.ReadInt32();
			v.overlayLayer = br.ReadInt32();
			v.glueLevel = br.ReadInt32();
			v.isUnique = br.ReadBoolean();
			v.dontRefreshDuration = br.ReadBoolean();
			v.cascadeMutators = br.ReadBoolean();
			v.removeMutatorIfNotMatching = br.ReadBoolean();
			v.mutationId = br.ReadBoolean() ? null : br.ReadString();
			v.countGlueAchievement = br.ReadBoolean();
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			multiplierField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_ProjectileBehaviorWithOverlayModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileBehaviorWithOverlayModel)m[i+start];
			v.overlayType = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_SlowForBloonModel_Fields(int start, int count) {
		Set_v_SlowModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.SlowForBloonModel)m[i+start];
			v.bloonId = br.ReadBoolean() ? null : br.ReadString();
			v.bloonIds = (Il2CppStringArray) m[br.ReadInt32()];
			v.bloonTag = br.ReadBoolean() ? null : br.ReadString();
			v.bloonTags = (Il2CppStringArray) m[br.ReadInt32()];
			v.excluding = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateProjectileOnExhaustFractionModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExhaustFractionModel)m[i+start];
			v.projectile = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.emission = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel) m[br.ReadInt32()];
			v.fraction = br.ReadSingle();
			v.durationfraction = br.ReadSingle();
			v.canCreateInBetweenRounds = br.ReadBoolean();
			v.collideOnSubProjectile = br.ReadBoolean();
		}
	}
	
	private void Set_v_AgeModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel)m[i+start];
			v.rounds = br.ReadInt32();
			v.useRoundTime = br.ReadBoolean();
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.endOfRoundClearBypassModel = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.EndOfRoundClearBypassModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_AddBehaviorToBloonModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorWithOverlayModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.AddBehaviorToBloonModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.AddBehaviorToBloonModel)m[i+start];
			v.mutationId = br.ReadBoolean() ? null : br.ReadString();
			v.layers = br.ReadInt32();
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.filter = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel) m[br.ReadInt32()];
			v.filters = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Bloons.BloonBehaviorModel>) m[br.ReadInt32()];
			v.isUnique = br.ReadBoolean();
			v.lastAppliesFirst = br.ReadBoolean();
			v.collideThisFrame = br.ReadBoolean();
			v.cascadeMutators = br.ReadBoolean();
			v.glueLevel = br.ReadInt32();
			v.applyOnlyIfDamaged = br.ReadBoolean();
			v.stackCount = br.ReadInt32();
			v.parentDamageModel = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_BloonBehaviorModelWithTowerTracking_Fields(int start, int count) {
		Set_v_BloonBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Bloons.BloonBehaviorModelWithTowerTracking)m[i+start];
		}
	}
	
	private void Set_v_BloonBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Bloons.BloonBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_DamageOverTimeModel_Fields(int start, int count) {
		Set_v_BloonBehaviorModelWithTowerTracking_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Bloons.Behaviors.DamageOverTimeModel>();
		var intervalField = t.GetField("interval", bindFlags);
		var initialDelayField = t.GetField("initialDelay", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Bloons.Behaviors.DamageOverTimeModel)m[i+start];
			v.damage = br.ReadSingle();
			v.payloadCount = br.ReadInt32();
			v.immuneBloonProperties = (BloonProperties) (br.ReadInt32());
			intervalField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.displayPath = ModContent.CreatePrefabReference(br.ReadString());
			v.displayLifetime = br.ReadSingle();
			v.triggerImmediate = br.ReadBoolean();
			v.rotateEffectWithBloon = br.ReadBoolean();
			initialDelayField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.damageOnDestroy = br.ReadBoolean();
			v.overrideDistributionBlocker = br.ReadBoolean();
			v.distributeToChildren = br.ReadBoolean();
			v.damageModifierModels = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Projectiles.DamageModifierModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateSoundOnProjectileExpireModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnProjectileExpireModel)m[i+start];
			v.sound1 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound3 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound4 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound5 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateEffectOnExhaustFractionModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExhaustFractionModel)m[i+start];
			v.assetId = ModContent.CreatePrefabReference(br.ReadString());
			v.lifespan = br.ReadSingle();
			v.fullscreen = br.ReadBoolean();
			v.fraction = br.ReadSingle();
			v.durationFraction = br.ReadSingle();
			v.randomRotation = br.ReadBoolean();
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_AbilityModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel>();
		var cooldownSpeedScaleField = t.GetField("cooldownSpeedScale", bindFlags);
		var animationOffsetField = t.GetField("animationOffset", bindFlags);
		var cooldownField = t.GetField("cooldown", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel)m[i+start];
			v.displayName = br.ReadBoolean() ? null : br.ReadString();
			v.description = br.ReadBoolean() ? null : br.ReadString();
			v.icon = ModContent.CreateSpriteReference(br.ReadString());
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Model>) m[br.ReadInt32()];
			v.activateOnPreLeak = br.ReadBoolean();
			v.activateOnLeak = br.ReadBoolean();
			v.addedViaUpgrade = br.ReadBoolean() ? null : br.ReadString();
			v.livesCost = br.ReadInt32();
			v.maxActivationsPerRound = br.ReadInt32();
			v.animation = br.ReadInt32();
			v.enabled = br.ReadBoolean();
			v.canActivateBetweenRounds = br.ReadBoolean();
			v.resetCooldownOnTierUpgrade = br.ReadBoolean();
			v.activateOnLivesLost = br.ReadBoolean();
			v.sharedCooldown = br.ReadBoolean();
			v.dontShowStacked = br.ReadBoolean();
			v.animateOnMainAttackDisplay = br.ReadBoolean();
			v.restrictAbilityAfterMaxRoundTimer = br.ReadBoolean();
			cooldownSpeedScaleField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			animationOffsetField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			cooldownField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_AbilityBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.AbilityBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_CreateEffectOnAbilityModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateEffectOnAbilityModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.randomRotation = br.ReadBoolean();
			v.centerEffect = br.ReadBoolean();
			v.destroyOnEnd = br.ReadBoolean();
			v.useAttackTransform = br.ReadBoolean();
			v.canSave = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateSoundOnAbilityModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateSoundOnAbilityModel)m[i+start];
			v.sound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_TurboModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.TurboModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.TurboModel)m[i+start];
			v.multiplier = br.ReadSingle();
			v.projectileDisplay = (Il2CppAssets.Scripts.Models.Effects.AssetPathModel) m[br.ReadInt32()];
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.extraDamage = br.ReadInt32();
			v.projectileRadiusScaleBonus = br.ReadSingle();
			v.dontRemoveMutatorOnDestroy = br.ReadBoolean();
		}
	}
	
	private void Set_v_AssetPathModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Effects.AssetPathModel)m[i+start];
			v.assetPath = ModContent.CreatePrefabReference(br.ReadString());
		}
	}
	
	private void Set_v_RemoveMutatorOnUpgradeModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.RemoveMutatorOnUpgradeModel)m[i+start];
			v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
			v.firstPath = br.ReadInt32();
			v.secondPath = br.ReadInt32();
			v.thirdPath = br.ReadInt32();
		}
	}
	
	private void Set_v_DamageUpModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.DamageUpModel)m[i+start];
			v.lifespanFrames = br.ReadInt32();
			v.additionalDamage = br.ReadInt32();
			v.projectileDisplay = (Il2CppAssets.Scripts.Models.Effects.AssetPathModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_RetargetOnContactModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.RetargetOnContactModel>();
		var delayField = t.GetField("delay", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.RetargetOnContactModel)m[i+start];
			v.distance = br.ReadSingle();
			v.maxBounces = br.ReadInt32();
			delayField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.targetType.id = br.ReadString();
			v.targetType.actionOnCreate = br.ReadBoolean();
			v.expireIfNoTargetFound = br.ReadBoolean();
		}
	}
	
	private void Set_v_TravelStraitModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel>();
		var speedField = t.GetField("speed", bindFlags);
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel)m[i+start];
			speedField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_OrbitModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.OrbitModel)m[i+start];
			v.projectile = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.count = br.ReadInt32();
			v.range = br.ReadSingle();
		}
	}
	
	private void Set_v_FilterAllModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterAllModel)m[i+start];
		}
	}
	
	private void Set_v_DontDestroyOnContinueModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DontDestroyOnContinueModel)m[i+start];
		}
	}
	
	private void Set_v_CantBeReflectedModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CantBeReflectedModel)m[i+start];
		}
	}
	
	private void Set_v_FilterMoabModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterMoabModel)m[i+start];
			v.flip = br.ReadBoolean();
		}
	}
	
	private void Set_v_CollideOnlyWithTargetModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CollideOnlyWithTargetModel)m[i+start];
		}
	}
	
	private void Set_v_IgnoreThrowMarkerModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.IgnoreThrowMarkerModel)m[i+start];
		}
	}
	
	#endregion
	
	protected override Il2CppAssets.Scripts.Models.Towers.TowerModel Load(byte[] bytes) {
		using (var s = new MemoryStream(bytes)) {
			using (var reader = new BinaryReader(s)) {
				this.br = reader;
				var totalCount = br.ReadInt32();
				m = new object[totalCount];
				
				//##  Step 1: create empty collections
				CreateArraySet<Il2CppAssets.Scripts.Models.Model>();
				Read_a_Int32_Array();
				Read_a_AreaType_Array();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Mods.ApplyModModel>();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel>();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>();
				Read_a_Vector3_Array();
				Read_a_String_Array();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				Read_a_TargetType_Array();
				Read_a_Single_Array();
				CreateArraySet<Il2CppAssets.Scripts.Models.Bloons.BloonBehaviorModel>();
				CreateListSet<Il2CppAssets.Scripts.Models.Model>();
				
				//##  Step 2: create empty objects
				Create_Records<Il2CppAssets.Scripts.Models.Towers.TowerModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Mods.ApplyModModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnSellModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Audio.SoundModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnUpgradeModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnSellModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Effects.EffectModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnPlaceModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnTowerPlaceModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnUpgradeModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmissionModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Filters.FilterInvisibleModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.FollowPathModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileFilterModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.RotateModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForTagModel>();
				Create_Records<Il2CppAssets.Scripts.Models.GenericBehaviors.DisplayModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.ClearHitBloonsModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.FlipFollowPathModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.AttackFilterModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCloseModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CircleFootprintModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Filters.FilterOutBloonModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.PushBackModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.ArriveAtTargetModel>();
				Create_Records<Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors.Curve>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Filters.FilterWithTagModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Filters.FilterOutTagModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.SlowForBloonModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExhaustFractionModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.AddBehaviorToBloonModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Bloons.Behaviors.DamageOverTimeModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnProjectileExpireModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExhaustFractionModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateEffectOnAbilityModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateSoundOnAbilityModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.TurboModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Effects.AssetPathModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.RemoveMutatorOnUpgradeModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.DamageUpModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.RetargetOnContactModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.OrbitModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Filters.FilterAllModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DontDestroyOnContinueModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CantBeReflectedModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Filters.FilterMoabModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CollideOnlyWithTargetModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.IgnoreThrowMarkerModel>();
				
				Set_v_TowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ApplyModModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnSellModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SoundModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnSellModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_EffectModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnPlaceModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnTowerPlaceModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AttackModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_WeaponModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SingleEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterInvisibleModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FollowPathModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ProjectileFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RotateModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageModifierForTagModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DisplayModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ClearHitBloonsModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FlipFollowPathModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RotateToTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AttackFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetFirstModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetLastModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetCloseModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetStrongModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CircleFootprintModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_UpgradePathModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterOutBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_PushBackModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ArriveAtTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_Curve_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterWithTagModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterOutTagModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SlowForBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateProjectileOnExhaustFractionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AgeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AddBehaviorToBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageOverTimeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnProjectileExpireModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnExhaustFractionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnAbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnAbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TurboModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AssetPathModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RemoveMutatorOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageUpModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RetargetOnContactModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TravelStraitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_OrbitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterAllModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DontDestroyOnContinueModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CantBeReflectedModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterMoabModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CollideOnlyWithTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_IgnoreThrowMarkerModel_Fields(br.ReadInt32(), br.ReadInt32());
				
				//##  Step 4: link object collections e.g Product[]. Note: requires object data e.g dictionary<string, value> where string = model.name
				LinkArray<Il2CppAssets.Scripts.Models.Model>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Mods.ApplyModModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Bloons.BloonBehaviorModel>();
				LinkList<Il2CppAssets.Scripts.Models.Model>();
				
				var resIndex = br.ReadInt32();
				UnityEngine.Debug.Assert(br.BaseStream.Position == br.BaseStream.Length);
				return (Il2CppAssets.Scripts.Models.Towers.TowerModel) m[resIndex];
			}
		}
	}
}
