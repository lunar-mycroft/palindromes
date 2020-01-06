from json.decoder import JSONDecodeError
from typing import Iterator, Tuple
from xml.dom import minidom

import requests
from requests.exceptions import HTTPError

difFormat = Exception("This json does not appear to be formated like the example given")

def getURL()->str:
    with open('App.config','r') as file:
        xml = minidom.parse(file)
        appSettings = xml.getElementsByTagName('appSettings')[0]
        items = appSettings.getElementsByTagName('add')
        for item in items:
            if item.attributes['key'].value=='url':
                return item.attributes['value'].value
    raise Exception("Could not find url in App.config")

def getStrings()->Iterator[str]:
    req = requests.get(getURL())
    try:
        req.raise_for_status()
        l = req.json()['strings']
        
    except HTTPError:
        raise Exception("There was some sort of problem downloading the json.  Are you sure you gave the correct url?")
    except JSONDecodeError:
        raise Exception("Invalid json.  Did you forget to link to a raw json file")
    except KeyError:
        raise difFormat

    for data in l:
        try:
            s = data['str']
            if not isinstance(s, str):
                raise difFormat
            yield s
        except KeyError:
            raise difFormat

def fromEnds(s: str)->Iterator[Tuple[str, str]]:
    i, j = 0, len(s)-1
    while i<=j:
        yield s[i], s[j]
        i, j = i+1, j-1

def prepString(s: str)->str:
    return ''.join(c.casefold() for c in s if c.isalnum())

def isPalindrome(s: str)->bool:
    s = prepString(s)
    return all(leading==trailing for (leading, trailing) in fromEnds(s))

if __name__=='__main__':
    for s in getStrings():
        print(f"{repr(s)} is {'' if isPalindrome(s) else 'not '}a palindrome", )