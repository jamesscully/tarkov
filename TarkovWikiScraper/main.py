import requests

from bs4 import BeautifulSoup
from Models import Item, HideoutUpgrade


def processQuestline(tokens: [str], itemName: str):
    if not len(tokens) > 0:
        return

    if tokens[0] == 'Quests':
        tokens.pop(0)

    print("\nProcessing questline: ", tokens)

    # ['in raid', ' for the quest ', 'Living high is not a crime - Part 1']
    amount = int(tokens.pop(0)[0])

    inRaid = False
    inRaidMessage = ""

    # ['in raid', ' for the quest ', 'Living high is not a crime - Part 1']
    # or
    # [' for the quest ', 'Living high is not a crime - Part 1']
    temp = tokens.pop(0)

    questName = "ERROR QUEST"
    inRaid = temp == 'in raid'

    # remove ' for the quest '
    if inRaid:
        tokens.pop(0)
        inRaidMessage = "in raid"

    try:
        questName = tokens.pop(0)
    except UnboundLocalError:
        print("Error with messages: ", tokens, " item name: ", itemName)
    except IndexError:
        print("Error with messages: ", tokens, " item name: ", itemName)

    print(f"Item [{itemName}] needs [{amount}] to be found [{inRaidMessage}] for the quest [{questName}]", tokens)

    if len(tokens) > 0:
        # print("\tRemaining messages: ", tokens)

        if tokens[0].__contains__("for the quest"):
            # print("\tFound extra quests")
            processQuestline(tokens, itemName)

    return


def processHideout(tokens: [str], item: Item):
    if tokens[0] == 'Hideout':
        tokens.pop(0)

    arr_upgrades: [HideoutUpgrade] = []

    # example: ['10 need to be found for the ', 'bitcoin farm level 1', '8 need to be found for the ', 'bitcoin farm
    # level 2', '5 need to be found for the ', 'generator level 3']

    # item name - amount - upgrade name

    amount = 0

    try:
        amount = int(tokens[0].split()[0])
        tokens.pop(0)
    except ValueError:
        return

    upgrade_name = tokens[0]

    tokens.pop(0)

    # print(amount, "of", itemName, "need to be found for", upgradeName)

    arr_upgrades.append(HideoutUpgrade(upgrade_name, newItem, amount))

    item.maxUpgrade += amount

    if len(tokens) > 0:
        for x in arr_upgrades:
            print("Returning ", str(x))
        arr_upgrades.append(processHideout(tokens, item))
    else:
        for x in arr_upgrades:
            print("Returning ", str(x))

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

        # should now be able to linearly process quests needed for
        # all that should be left is 'Quests' or 'Hideout', indcating quest item or needed  for upgrades
        # example: ['Quests', '1 needs to be found ', 'in raid', ' for the quest ', 'Collector']

        if len(messages) == 0:
            continue

        if len(messages) > 0 and messages[0] == 'Quests':
            newItem.isQuestItem = True
            processQuestline(messages, newItem)
            messages.pop(0) if messages else None

        if len(messages) > 0 and messages[0] == 'Hideout':
            newItem.isHideoutItem = True
            item, upgrades = processHideout(messages, newItem)

            messages.pop(0) if messages else None



