# LexoRank on C#
A reference implementation of a list ordering system like [JIRA's Lexorank algorithm](https://www.youtube.com/watch?v=OjQv9xMoFbg).

[![Build Status](https://travis-ci.org/kvandake/lexorank-dotnet.svg?branch=master)](https://travis-ci.org/kvandake/lexorank-dotnet) [![NuGet version](https://badge.fury.io/nu/LexoRank.svg)](https://badge.fury.io/nu/LexoRank)


## Getting Started

> Install-Package LexoRank

## Using

### Static methods


```cs
using LexoAlgorithm;

// min
const minLexoRank = LexoRank.Min();
// max
const maxLexoRank = LexoRank.Max();
// middle
const maxLexoRank = LexoRank.Middle();
// parse
const parsedLexoRank = LexoRank.Parse('0|0i0000:');
```

### Public methods

```cs
using LexoAlgorithm;

// any lexoRank
const lexoRank = LexoRank.Middle();

// generate next lexorank
const nextLexoRank = lexoRank.GenNext();

// generate previous lexorank
const prevLexoRank = lexoRank.GenPrev();

// ToString
const lexoRankStr = lexoRank.ToString();
```

### Calculate LexoRank

LexRank calculation based on existing LexoRanks.
```cs
using LexoAlgorithm;

// any lexorank
const any1LexoRank = LexoRank.Min();
// another lexorank
const any2LexoRank = any1LexoRank.GenNext().GenNext();
// calculate between
const betweenLexoRank = any1LexoRank.Between(any2LexoRank);
```

## Related projects
- [LexoRank on Typescript](https://github.com/kvandake/lexorank-ts)

## Licence
MIT

---
**I have not found information about the license of the algorithm LexoRank. 
If the rights are violated, please contact me to correct the current license.**
