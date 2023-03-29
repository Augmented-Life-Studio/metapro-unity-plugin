require('@nomiclabs/hardhat-waffle')
require('@nomiclabs/hardhat-web3')
require('@nomiclabs/hardhat-etherscan')
require('solidity-coverage')
require('hardhat-deploy')
require('dotenv').config()
require('hardhat-gas-reporter')

// This is a sample Hardhat task. To learn how to create your own go to
// https://hardhat.org/guides/create-task.html
task('accounts', 'Prints the list of accounts', async (taskArgs, hre) => {
	const accounts = await hre.ethers.getSigners()

	for (const account of accounts) {
		console.log(account.address)
	}
})

// You need to export an object to set up your config
// Go to https://hardhat.org/config/ to learn more

/**
 * @type import('hardhat/config').HardhatUserConfig
 */

// const MNEMONIC = process.env.MNEMONIC || 'sample-mnemonic'
// const BSCSCAN_API_KEY = process.env.BSCSCAN_API_KEY || 'etherscan-api-key'

module.exports = {
	gasReporter: {
		currency: 'USD',
		token: 'BNB',
		gasPriceApi: 'https://api.bscscan.com/api?module=proxy&action=eth_gasPrice',
		gasPrice: 5,
		coinmarketcap: '48fdecb5-5c9f-4424-8f58-c985679e3b90',
		enabled: process.env.GAS_REPORT ? true : false,
		// enabled: true,
	},
	solidity: {
		version: '0.8.4',
		settings: {
			optimizer: {
				enabled: true,
				runs: 200,
			},
		},
	},
	namedAccounts: {
		deployer: 0,
	},
	mocha: {
		timeout: 20000,
	},
}
