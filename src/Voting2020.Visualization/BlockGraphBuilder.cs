using System;
using System.Linq;

using Voting2020.Core;

namespace Voting2019.Visualization
{
	public static class BlockGraphBuilder
	{
		public static BlockGraphItem<TimeSpan>[] BlockStart<T>(
			T[] data,
			Func<T,int> blockNumberSelector,
			Func<T,TimeSpan> blockTimestampSelector)
		{
			var blocks = data.Select(x => new BlockGraphItem<TimeSpan>(
				blockNumberSelector(x), blockTimestampSelector(x)))
				.Distinct()
				.OrderBy(x => x.BlockNumber)
				.ToArray();
			return blocks;
		}

		public static BlockGraphItem<TimeSpan>[] BlockTime<T>(
			T[] data,
			Func<T, int> blockNumberSelector,
			Func<T, TimeSpan> blockTimestampSelector)
		{
			var blocks = BlockStart(data, blockNumberSelector, blockTimestampSelector);


			BlockGraphItem<TimeSpan>[] ret = new BlockGraphItem<TimeSpan>[blocks.Length - 1];

			TimeSpan lastBlockStartTime = blocks[0].Data;
			int lastBlockNumber = blocks[0].BlockNumber;

			for (int i = 1; i < blocks.Length; i++)
			{
				ref BlockGraphItem<TimeSpan> currentBlock = ref blocks[i];

				var blocksTime = currentBlock.Data - lastBlockStartTime;
				var numberOfBlocks = currentBlock.BlockNumber - lastBlockNumber;

				var averageBlockTime = blocksTime / numberOfBlocks;

				ret[i - 1] = new BlockGraphItem<TimeSpan>(currentBlock.BlockNumber, averageBlockTime);

				lastBlockStartTime = currentBlock.Data;
				lastBlockNumber = currentBlock.BlockNumber;
			}

			return ret;
		}

		public static BlockGraphItem<int>[] TransactionPerBlock<T>(
			T[] data,
			Func<T, int> blockNumberSelector)
		{
			var points = data
					.GroupBy(x => blockNumberSelector(x), (key, votes) => new BlockGraphItem<int>(key, votes.Count()))
					.OrderBy(x => x.BlockNumber)
					.ToArray();
			return points;
		}
	}
}