# RandomizedTest
Demo of Randomized Response model

Suppose you want to estimate the proportion of cheaters among students. Nobody will confess that he/she cheated. The randomized response model offers the plausible deniability for test takers.

The process goes as follows:
- Student comes up with an answer (cheating or working it through himself; this is hidden)
- Student tosses two fair coins
  - if coin #1 is heads, he faitfully reports whether cheating took place
  - else:
    - if coin #2 is heads: report cheating
    - else: report no cheating

The program generates synthetic dataset given number of examples and the true cheating proportion.

The program code is BSD licensed but uses the Infer.NET library.
The Infer.NET library has its own, more restricted license. Check the terms on the respective website.
