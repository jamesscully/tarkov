import sys
import requests
from bs4 import BeautifulSoup, element

from Item import Item

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
        messages = list(filter(('\n').__ne__, messages))

        if messages.__contains__('Barter Item'):
            newItem.isBarterItem = True

        if messages.__contains__('Crafting item') or messages.__contains__('Crafting item'):
            newItem.isCraftItem = True

        if messages.__contains__('Quests'):
            newItem.isQuestItem = True

            # print("Messages found: ", messages)
            # print(f"-> End of examination {newItem.name}, \n\tcraft: {newItem.isCraftItem} \n\tbarter: {newItem.isBarterItem}\n\tquest: {newItem.isQuestItem}\n\n")

        # print("Item: {} is type '{}' craft: {} barter: {} ".format(newItem.name, newItem.type, newItem.isCraftItem,
        #       newItem.isBarterItem))
