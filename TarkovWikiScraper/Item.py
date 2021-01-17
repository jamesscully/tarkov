class Item:
    description: str = "DEFAULT DESCRIPTION"
    type : str = "DEFAULT TYPE"
    notes : str = "DEFAULT NOTES"
    locations : [str] = ["DEFAULT LOCATIONS"]
    quests : [str] = ["DEFAULT QUESTS"]

    isBarterItem = False
    isCraftItem = False
    isQuestItem = False

