from typing import Iterator, Tuple
from requests import get

def fromEnds(s: str)->Iterator[Tuple[str, str]]:
    i, j = 0, len(s)-1
    while i<=j:
        yield s[i], s[j]
        i, j = i+1, j-1

def isPalindrome(s: str)->bool:
    s = s.replace(' ', '').casefold()
    return all(leading==trailing for (leading, trailing) in fromEnds(s))


data = get('https://raw.githubusercontent.com/bungard/PalindromeTest/master/string.json').json()
for d in data['strings']:
    print(repr(d['str']), isPalindrome(d['str']))
