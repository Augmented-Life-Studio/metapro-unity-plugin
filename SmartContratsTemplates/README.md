# NFT Bundler

## Run Tests

Compile solidity contracts and run the test suite with:

```shell
npx hardhat test test/MetaproINS.js
```

## Deploy

```shell
yarn deploy --network kovan
```

MetaproINS
npx hardhat run deploy/deployINS.js --network testnet
npx hardhat verify -_tokenAddress- -_busdAddress- -_treasuryAddress- --contract contracts/MetaproINS.sol:MetaproINS --network testnet
## Hardhat Commands

```shell
npx hardhat run deploy/deployMetaproRoyalty.js --network testnet
```
## Scripts
To run scripts properly we need to install higher version of ethers package ^6.0.0