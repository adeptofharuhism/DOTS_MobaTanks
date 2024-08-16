public enum Model : short { MeleeMobAnimated, RangedMobAnimated }
public static class ModelSize { public const short Size = 2; }

public enum MeleeMobAnimated : byte { MeleeMob_Idle, MeleeMob_Attack, MeleeMob_Run, MeleeMob_Death }
public static class MeleeMobAnimatedSize { public const byte Size = 4; }
public enum RangedMobAnimated : byte { RangedMob_Idle, RangedMob_Attack, RangedMob_Run, RangedMob_Death }
public static class RangedMobAnimatedSize { public const byte Size = 4; }
