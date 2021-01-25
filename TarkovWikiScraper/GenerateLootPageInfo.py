import requests
import re

from bs4 import BeautifulSoup
from Models import Item, HideoutUpgrade, QuestItem


def parseQuestline(item: Item, tokens: [str]):

    # used to exit this function graciously; return item but no quests
    empty = (item, [])

    if not len(tokens) > 0:
        return empty

    if item.name == "Dogtag" or item.name == "Paracord":
        return empty

    print("\tParsing quests", tokens)

    quests: [QuestItem] = []

    while len(tokens) > 0:

        # if we somehow have 'quests' or hideout identifier, just ignore it
        if tokens[0] == 'Quests':
            tokens.pop(0)

        if tokens[0] == 'Hideout':
            break

        quest = QuestItem()

        firSkip = False

        try:
            first_word: str = tokens.pop(0)
            quest.questAmount = int(first_word.split()[0])

            # some items glob two tokens together if not needed to be found in raid, this will fix .pop orders
            if re.match("[0-9]+ needs to be found for the quest", first_word):
                firSkip = True
                print("Skipped found in raid check")

        except ValueError as e:
            print("Error item:", item.name)
            item.hasError = True
            return empty

        if not firSkip:
            # ['in raid', ' for the quest ', 'Living high is not a crime - Part 1']
            # or
            # [' for the quest ', 'Living high is not a crime - Part 1']
            temp = tokens.pop(0)

            quest.bFoundInRaid = temp == 'in raid'

            # remove ' for the quest '
            if quest.bFoundInRaid:
                tokens.pop(0)

        try:
            quest.questName = tokens.pop(0)
        except UnboundLocalError:
            print("\t\tError with messages: ", tokens, " item name: ", item.name)
        except IndexError:
            print("\t\tError with messages: ", tokens, " item name: ", item.name)

        quests.append(quest)

    return item, quests


def parseHideout(item: Item, tokens: [str], toplevel=True):
    if tokens[0] == 'Hideout':
        tokens.pop(0)

    if toplevel:
        print("\tParsing hideout upgrades:")

    arr_upgrades: [HideoutUpgrade] = []

    # example: ['10 need to be found for the ', 'bitcoin farm level 1']

    while len(tokens) > 0:
        upgrade = HideoutUpgrade()

        try:
            first_word = tokens.pop(0).split()[0]  # amount should be the first 'word'
            upgrade.upgradeAmount = int(first_word)
        except ValueError:
            return item, []

        upgrade.upgradeName = str(tokens.pop(0))  # upgrade name should be first after prev pop

        arr_upgrades.append(upgrade)

        item.maxUpgrade += upgrade.upgradeAmount

        print(f"\t\t Item requires {upgrade.upgradeAmount} for {upgrade.upgradeName}")

    return item, arr_upgrades


if __name__ == '__main__':
    url = requests.get('https://escapefromtarkov.gamepedia.com/Loot').text

    soup = BeautifulSoup(url, 'html.parser')
    table = soup.find('table', {'class': 'wikitable sortable'})
    images = table.findAll('img')

    for row in table.findAll('tr'):
        newItem = Item()

        # ignore header of table
        if row.find('th').find('a') is None:
            continue

        # wiki uses <th> for icon/name, <td> for item type/notes
        iconAndName = row.find_all('th')
        typeAndNotes = row.find_all('td')

        newItem.name = iconAndName[1].a.contents[0]
        newItem.type = typeAndNotes[0].contents[0].rstrip()

        print(f"\nProcessing `{newItem.name} ` {{")

        notes = typeAndNotes[1]

        messages = []

        # add all text we find to 'messages', i.e. 'Quests' 'Barter Item'
        for x in notes.find_all(text=True):
            messages.append(str(x))

        # strip un-needed '\n' linebreaks
        messages = list(filter('\n'.__ne__, messages))

        if messages.__contains__('Barter Item'):
            newItem.isBarterItem = True

            # remove 'barter item' from messages
            try:
                messages.remove('Barter Item')
            except ValueError:
                pass

        if messages.__contains__('Crafting item') or messages.__contains__('Crafting item'):
            newItem.isCraftItem = True

            # remove our 'crafting item' from messages
            try:
                messages.remove('Crafting Item')
            except ValueError:
                pass
            try:
                messages.remove('Crafting item')
            except ValueError:
                pass

        print("\tBarter Item:", newItem.isBarterItem)
        print("\tCraft Item: ", newItem.isCraftItem)

        # should now be able to linearly process quests needed for
        # all that should be left is 'Quests' or 'Hideout', indcating quest item or needed  for upgrades
        # example: ['Quests', '1 needs to be found ', 'in raid', ' for the quest ', 'Collector']
        if len(messages) > 0 and messages[0] == 'Quests':
            newItem.isQuestItem = True
            newItem, newItem.quests = parseQuestline(newItem, messages)

            messages.pop(0) if messages else None

        if len(messages) > 0 and messages[0] == 'Hideout':
            newItem.isHideoutItem = True
            newItem, newItem.upgrades = parseHideout(newItem, messages)

            messages.pop(0) if messages else None

        if newItem.isHideoutItem:
            print("\tHideout Upgrades: ")
            for y in newItem.upgrades:
                print("\t\t", y)

        if newItem.isQuestItem and newItem.quests is not None:
            print("\tNeeded for Quests: ")
            for q in newItem.quests:
                print("\t\t", q)

        if newItem.hasError:
            print(f"###### Error in {newItem.name} #####")

        print(f"}}")
