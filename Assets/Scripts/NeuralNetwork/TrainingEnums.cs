public enum ResourceType {
	HumansCount, //+
	AvailableHumans, //+
	SpendingOnHousing, //+
	SpendingOnFields, //+
	SpendingOnInstruments, //+
	SpendingOnFood, //+
	SpendingOnForest, //+
	SpendingOnClay, //+
	SpendingOnStone, //+
	SpendingOnGold, //+
	SpendingOnBuilding1, //+
	SpendingOnBuilding2, //+
	SpendingOnBuilding3, //+
	SpendingOnBuilding4, //+
	SpendingOnCard1, //+
	SpendingOnCard2, //+
	SpendingOnCard3, //+
	SpendingOnCard4, //+

	SpentOnHousing, //+
	SpentOnFields, //+
	SpentOnInstruments, //+
	SpentOnFood, //+
	SpentOnForest, //+
	SpentOnClay, //+
	SpentOnStone, //+
	SpentOnGold, //+
	SpentOnBuilding1, //+
	SpentOnBuilding2, //+
	SpentOnBuilding3, //+
	SpentOnBuilding4, //+
	SpentOnCard1, //+
	SpentOnCard2, //+
	SpentOnCard3, //+
	SpentOnCard4, //+
	Charity,

	DicePoints,
	Food, //+
	Forest, //+
	Clay, //+
	Stone, //+
	Gold, //+
	Fields, //+
	Instruments, //+
	InstrumentsAvailable, //+
	InstrumentsOnce, //+
	HumanMultiplier, //+
	InstrumentsMultiplier, //+
	HousesMultiplier, //+
	FieldsMultiplier, //+
	HousesCount, //+
	SciencesIn1stLine, //+
	SciencesIn2ndLine, //+
	Any2ResourcesFromCard,
	UnspentFields, //+
	Score, //


	//////////////
	OneCardBottomMore
}
public enum ModelChangeType {
	StartGame,  // +food +humanscount
	StartRound, // +availablehumans
	EndTurn,

	AddHumansFromHousing, // +availablehumans
	AddAvailableInstruments,
	AddUnspentFields,

	WhereToGoSelected,
	SetSpentHumans, // -availablehumans +SpentOnHousing +SpentOnFields +SpentOnInstruments +SpentOnFood +SpentOnForest

	ApplyGoToInstruments, // -SpentOnInstruments 
	ApplyGoToFields, // -SpentOnFields
	ApplyGoToHousing, // +humanscount -SpentOnHousing
	ApplyGoToHouse, // -forest -clay -stone -gold
	ApplyGoToCard, // -forest -clay -stone -gold

	ApplyGoToMining,
	ApplyingInstruments,
	ResourcesMining, // +food +forest +clay +stone +gold -SpentOnFood -SpentOnForest

	ReceiveAny2ResFromCard,
	BonusFromOwnCharity, // +forest +clay +stone +gold
	BonusFromOthersCharity, // +forest +clay +stone +gold
	ApplyCardFromOtherCard,

	Feeding // -food
}