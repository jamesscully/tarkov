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


class HideoutUpgrade:
    name: str
    item: Item
    amount: int

    def __init__(self, name: str, item: Item, amount: int):
        self.name = name
        self.item = item
        self.amount = amount

    def __str__(self):
        return "{} of {} needed for {}".format(self.amount, self.item.name, self.name)
