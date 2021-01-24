class Item:
    description: str = "DEFAULT DESCRIPTION"
    type: str = "DEFAULT TYPE"
    notes: str = "DEFAULT NOTES"
    locations: [str] = ["DEFAULT LOCATIONS"]
    quests: [str] = ["DEFAULT QUESTS"]
    upgrades: []

    name: str = "DEFAULT NAME"

    maxUpgrade: int = 0
    maxQuest: int = 0

    isBarterItem = False
    isCraftItem = False
    isQuestItem = False
    isHideoutItem = False

    qNeedInRaid = False
    qAmount = -1

    def __init__(self):
        pass

    def __str__(self):
        return self.name


class QuestItem:
    questName: str = ""
    questAmount: int = -1
    bFoundInRaid: bool = False


class HideoutUpgrade:
    upgradeName: str
    upgradeAmount: int

    def __init__(self, name: str = "", amount: int = -1):
        self.upgradeName = name
        self.upgradeAmount = amount

    def __str__(self):
        return "{} needed for {}".format(self.upgradeAmount, self.upgradeName)
