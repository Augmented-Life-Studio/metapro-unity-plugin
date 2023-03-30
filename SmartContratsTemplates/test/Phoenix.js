const {expect} = require('chai')
const {BigNumber} = require('ethers')
const {ethers} = require('hardhat')
const {mine} = require('@nomicfoundation/hardhat-network-helpers')
const _ = require('lodash')

// test command => npx hardhat test test/Phoenix.js

describe('Phoenix', () => {
	let metaproMetaAsset
	let phoenix

	let deployer
	let treasury
	let questCompleter
	let accounts
	let transaction

	// Quest creator tokens
	let outputTokenIds = []

	// Quest completer tokens
	let inputTokenIds = []

	before(async () => {
		const allSigners = await ethers.getSigners()
		const [
			contractDeployer,
			contractQuestCompleter,
			contractTreasury,
			...restAccounts
		] = allSigners

		deployer = contractDeployer
		treasury = contractTreasury
		questCompleter = contractQuestCompleter
		accounts = restAccounts

		//Create mocked BUSD contract
		const BusdToken = await ethers.getContractFactory('BUSDToken')
		transaction = await ethers.provider._getBlock()
		currentBlockNumber = transaction.number
		busd = await BusdToken.deploy([questCompleter.address])
		transaction = await busd.balanceOf(deployer.address)
		expect(BigNumber.from(transaction).gt(BigNumber.from(0))).to.be.true

		transaction = await busd.balanceOf(questCompleter.address)
		expect(BigNumber.from(transaction).gt(BigNumber.from(0))).to.be.true
		//Create MetaproMetaAsset contract
		const MetaproMetaAsset = await ethers.getContractFactory('MetaAsset')
		metaproMetaAsset = await MetaproMetaAsset.deploy(
			'metaproUri',
			treasury.address,
		)

		// Create Phoenix contract
		const Phoenix = await ethers.getContractFactory('Phoenix')
		phoenix = await Phoenix.deploy()
		// Approve level1Referrer for spending busd

		// Check deployment of the MetaproMetaAsset
		expect(await metaproMetaAsset.treasuryAddress()).to.equal(treasury.address)

		//Mint token for the quest creator
		const tokenSupply = 1000

		const expectedCreatorTokenIds = [1001, 1002, 1003]
		const creatorMintPromises = await Promise.all(
			expectedCreatorTokenIds.map(el => {
				outputTokenIds.push(el)
				return metaproMetaAsset
					.connect(deployer)
					.create(deployer.address, 1000, `bucketHash${el}`, 0x00)
			}),
		)

		await Promise.all(
			creatorMintPromises.map(transaction => {
				transaction.wait()
			}),
		)

		expectedCreatorTokenIds.forEach(async el => {
			expect(await metaproMetaAsset.balanceOf(deployer.address, el)).to.equal(
				tokenSupply,
			)
		})

		//Mint token for the quest completer

		const expectedCompleterTokenIds = [1004, 1005, 1006]
		const completerMintPromises = await Promise.all(
			expectedCompleterTokenIds.map(el => {
				inputTokenIds.push(el)
				return metaproMetaAsset
					.connect(questCompleter)
					.create(questCompleter.address, 1000, `bucketHash${el}`, 0x00)
			}),
		)

		await Promise.all(
			completerMintPromises.map(transaction => {
				transaction.wait()
			}),
		)

		expectedCompleterTokenIds.forEach(async el => {
			expect(
				await metaproMetaAsset.balanceOf(questCompleter.address, el),
			).to.equal(tokenSupply)
		})

		//Approve Phoenix spending on MetaproMetaAssets
		transaction = await metaproMetaAsset
			.connect(deployer)
			.setApprovalForAll(phoenix.address, true)
		await transaction.wait()

		transaction = await metaproMetaAsset
			.connect(questCompleter)
			.setApprovalForAll(phoenix.address, true)
		await transaction.wait()
	})

	describe('Create Phoenix quest', () => {
		it('Create Phoenix quest successfully', async () => {
			// QuestId = 1 - Single participation
			transaction = await phoenix.connect(deployer).create(
				metaproMetaAsset.address,
				10,
				inputTokenIds,
				outputTokenIds,
				inputTokenIds.map(() => 1),
				outputTokenIds.map(() => 2),
				false,
				ethers.provider.blockNumber,
				ethers.provider.blockNumber + 200,
				'0x00',
			)

			await transaction.wait()

			let error
			try {
				transaction = await phoenix.connect(deployer).closeQuest(1, '0x00')
				await transaction.wait()
			} catch (err) {
				error = err
			}
			expect(error).to.not.be.empty
			// TaskId = 2 - Multiple participation
			transaction = await phoenix.connect(deployer).create(
				metaproMetaAsset.address,
				2,
				inputTokenIds,
				outputTokenIds,
				inputTokenIds.map(() => 1),
				outputTokenIds.map(() => 2),
				true,
				ethers.provider.blockNumber,
				ethers.provider.blockNumber + 200,
				'0x00',
			)

			await transaction.wait()
		})
		it('Saves proper configuration on quest', async () => {
			const [
				_questId,
				_tokenContractAddress,
				_totalNumberOfQuests,
				_numberOfQuestsCompleted,
				,
				,
				,
				,
				_operator,
				_multipleParticipation,
				_startBlock,
				_endBlock,
				_valid,
			] = await phoenix.getQuestById(1)
			expect(_questId).to.equal(BigNumber.from(1))
			expect(_tokenContractAddress).to.equal(metaproMetaAsset.address)
			expect(_totalNumberOfQuests).to.equal(BigNumber.from(10))
			expect(_numberOfQuestsCompleted).to.equal(BigNumber.from(0))
			expect(_operator).to.equal(deployer.address)
			expect(_multipleParticipation).to.false
			expect(_endBlock.sub(_startBlock)).to.equal(BigNumber.from(200))
			expect(_valid).to.true
		})
	})

	describe('Testing unhappy paths on create', () => {
		it('Should revert create transaction when one of input/output tokens have negative id', async () => {
			try {
				transaction = await phoenix.connect(deployer).create(
					metaproMetaAsset.address,
					10,
					inputTokenIds,
					outputTokenIds,
					inputTokenIds.map(tokenId => tokenId - 2000),
					outputTokenIds.map(() => 2),
					false,
					ethers.provider.blockNumber,
					ethers.provider.blockNumber + 200,
					'0x00',
				)

				await transaction.wait()
			} catch (error) {
				expect(error).to.not.be.empty
			}
			try {
				transaction = await phoenix.connect(deployer).create(
					metaproMetaAsset.address,
					10,
					inputTokenIds,
					outputTokenIds,
					inputTokenIds.map(() => 1),
					outputTokenIds.map(tokenId => tokenId - 2000),
					false,
					ethers.provider.blockNumber,
					ethers.provider.blockNumber + 200,
					'0x00',
				)

				await transaction.wait()
			} catch (error) {
				expect(error).to.not.be.empty
			}
		})
		it('Should revert create transaction when startBlock is greater than endBlock', async () => {
			try {
				transaction = await phoenix.connect(deployer).create(
					metaproMetaAsset.address,
					10,
					inputTokenIds,
					outputTokenIds,
					inputTokenIds.map(() => 1),
					outputTokenIds.map(() => 1),
					false,
					100,
					50,
					'0x00',
				)

				await transaction.wait()
			} catch (error) {
				expect(error).to.not.be.empty
			}
		})
		it('Should revert create transaction when endBlock is lower than startBlock', async () => {
			try {
				transaction = await phoenix.connect(deployer).create(
					metaproMetaAsset.address,
					10,
					inputTokenIds,
					outputTokenIds,
					inputTokenIds.map(() => 1),
					outputTokenIds.map(() => 1),
					false,
					0,
					0,
					'0x00',
				)

				await transaction.wait()
			} catch (error) {
				expect(error).to.not.be.empty
			}
		})
	})

	describe('Quest participation in single participable quest', () => {
		it('Correctly participate in quest', async () => {
			transaction = await phoenix
				.connect(questCompleter)
				.completeQuest(1, '0x00')
			await transaction.wait()
		})
		it('Saves proper configuration on completed quest', async () => {
			const [, , , _numberOfQuestsCompleted] = await phoenix.getQuestById(1)
			expect(_numberOfQuestsCompleted).to.equal(BigNumber.from(1))
		})
		it('Participate in quest second time with error', async () => {
			try {
				transaction = await phoenix
					.connect(questCompleter)
					.completeQuest(1, '0x00')
				await transaction.wait()
			} catch (error) {
				expect(error).to.not.be.empty
			}
		})
	})
	describe('Quest participation in multiple participable quest', () => {
		it('Correctly participate in quest', async () => {
			transaction = await phoenix
				.connect(questCompleter)
				.completeQuest(2, '0x00')
			await transaction.wait()
		})
		it('Saves proper configuration on completed quest', async () => {
			const [, , , _numberOfQuestsCompleted] = await phoenix.getQuestById(1)
			expect(_numberOfQuestsCompleted).to.equal(BigNumber.from(1))
		})
		it('Participate in quest second time with success', async () => {
			transaction = await phoenix
				.connect(questCompleter)
				.completeQuest(2, '0x00')
			await transaction.wait()
		})
		it('Saves proper configuration on second completed quest', async () => {
			const questConfiguration = await phoenix.getQuestById(2)
			const [, , , _numberOfQuestsCompleted] = questConfiguration
			expect(_numberOfQuestsCompleted).to.equal(BigNumber.from(2))
		})
		it('Participate in quest third time with error', async () => {
			try {
				transaction = await phoenix
					.connect(questCompleter)
					.completeQuest(2, '0x00')
				await transaction.wait()
			} catch (error) {
				expect(error).to.not.be.empty
			}
		})
	})
	describe('Check if time frame for third quest is valid', () => {
		it('Suppose to throw error when quest is not started', async () => {
			transaction = await phoenix.connect(deployer).create(
				metaproMetaAsset.address,
				10,
				inputTokenIds,
				outputTokenIds,
				inputTokenIds.map(() => 1),
				outputTokenIds.map(() => 2),
				4,
				ethers.provider.blockNumber + 100,
				ethers.provider.blockNumber + 200,
				'0x00',
			)
			await transaction.wait()
			try {
				transaction = await phoenix
					.connect(questCompleter)
					.completeQuest(3, '0x00')
				await transaction.wait()
			} catch (error) {
				expect(error).to.not.be.empty
			}
		})
		it('Suppose to go through when is on time frame', async () => {
			await mine(100)
			transaction = await phoenix
				.connect(questCompleter)
				.completeQuest(3, '0x00')
			await transaction.wait()
		})
		it('Suppose to throw error when quest is finished', async () => {
			await mine(1000)
			try {
				transaction = await phoenix
					.connect(questCompleter)
					.completeQuest(3, '0x00')
				await transaction.wait()
			} catch (error) {
				expect(error).to.not.be.empty
			}
		})
	})
})
