namespace GlobalDefine
{
	#region Monster
	public enum eMonsterState
	{
		None = 0,
		Idle,
		Move,
		Damage,
		Attack,
		SkillAttack,
		Dead,
		Max,
	}
	public enum eMonsterMove
	{
		None = 0,
		Move,
		NotMove,
	}
	public enum eMonsterAttackType
	{
		None = 0,
		Melee,
		Range,
	}
	#endregion
}