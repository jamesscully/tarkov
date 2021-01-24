class QuestItem:
    questName: str = ""
    questAmount: int = -1
    bFoundInRaid: bool = False

    def __str__(self):
        foundInRaidMessage = "*found in raid*" if self.bFoundInRaid else ""
        return "{} needed {} for quest {}".format(self.questAmount, foundInRaidMessage, self.questName)


class HideoutUpgrade:
    upgradeName: str
    upgradeAmount: int

    def __init__(self, name: str = "", amount: int = -1):
        self.upgradeName = name
        self.upgradeAmount = amount

    def __str__(self):
        return "{} needed for {}".format(self.upgradeAmount, self.upgradeName)


class Item:
    description: str = "DEFAULT DESCRIPTION"
    type: str = "DEFAULT TYPE"
    notes: str = "DEFAULT NOTES"
    locations: [str] = ["DEFAULT LOCATIONS"]
    quests: [QuestItem] = []
    upgrades: []

    name: str = "DEFAULT NAME"

    maxUpgrade: int = 0
    maxQuest: int = 0

    isBarterItem = False
    isCraftItem = False
    isQuestItem = False
    isHideoutItem = False

    hasError: bool = False

    def __init__(self):
        pass

    def __str__(self):
        return self.name
