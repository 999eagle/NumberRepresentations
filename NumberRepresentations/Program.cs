using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberRepresentations
{
	class Program
	{
		static void Main(string[] args)
		{
			var goal = Search(1);
			Console.Write("goal found: ");
			var text = String.Join(" ", goal[1].Select(t => (char)t.symbol));
			Console.WriteLine(text);
			Console.ReadLine();
		}
		static IDictionary<int, Token[]> Search(int maxGoal)
		{
			var openList = new[]
			{
				new Token[0]
			};
			Token[][] nextOpenList = null;
			var foundNodes = new Dictionary<int, Token[]>();
			int maxDepth = 0;
			ulong currentDepthNodes = 1;
			ulong nextDepthNodeCount = 6;
			while (foundNodes.Count < maxGoal)
			{
				nextOpenList = new Token[nextDepthNodeCount][];
				ulong nextListIndex = 0;
				nextDepthNodeCount = 0;
				for (ulong i = 0; i < currentDepthNodes; i++)
				{
					var node = openList[i];
					if (node == null)
						break;
					var children = GeneratePossibleChildren(node);
					foreach (var child in children)
					{
						if (child == null) continue;
						if (GetNextNumber(child) == 10 && GetStackCount(child) == 1)
						{
							int val = GetStack(child)[0];
							if (val >= 1 && val <= maxGoal)
							{
								if (!foundNodes.ContainsKey(val))
								{
									foundNodes.Add(val, child);
								}
								continue;
							}
						}
						nextOpenList[nextListIndex++] = child;
						nextDepthNodeCount += (ulong)CountPossibleChildren(child);
					}
					if (i % 10000 == 0)
					{
						Console.WriteLine($"Processed {i} / {currentDepthNodes} nodes at depth {maxDepth}.");
					}
				}
				maxDepth++;
				currentDepthNodes = nextListIndex;
				openList = nextOpenList;
			}
			return foundNodes;
		}

		static byte CountPossibleChildren(Token[] currentExpr)
		{
			byte count = 0;
			if (GetStackCount(currentExpr) >= 2)
			{
				count += 5;
			}
			if (GetNextNumber(currentExpr) <= 9)
			{
				count++;
			}
			return count;
		}

		static Token[][] GeneratePossibleChildren(Token[] currentExpr)
		{
			var children = new List<Token>();
			if (GetStackCount(currentExpr) >= 2)
			{
				children.Add(new Token { symbol = (byte)'+' });
				children.Add(new Token { symbol = (byte)'-' });
				children.Add(new Token { symbol = (byte)'*' });
				//children.Add(new Token { symbol = (byte)'/' });
				children.Add(new Token { symbol = (byte)'^' });
				children.Add(new Token { symbol = (byte)'|' });
			}
			byte nextNumber = GetNextNumber(currentExpr);
			if (nextNumber <= 9)
			{
				children.Add(new Token { symbol = (byte)(nextNumber + '0') });
			}
			return children.Select(t => currentExpr.Concat(new[] { t }).ToArray()).ToArray();
		}

		static int GetStackCount(Token[] tokens)
		{
			int count = 0;
			foreach (var token in tokens)
			{
				if (token.symbol >= '0' && token.symbol <= '9')
				{
					count++;
				}
				else
				{
					count--;
				}
			}
			return count;
		}

		static byte GetNextNumber(Token[] tokens)
		{
			if (tokens.Length == 0) return 1;
			for (byte i = (byte)(tokens.Length - 1); i >= 0; i--)
			{
				if (tokens[i].symbol >= '0' && tokens[i].symbol <= '9')
					return (byte)((tokens[i].symbol - '0') + 1);
			}
			return 1;
		}

		static int[] GetStack(Token[] tokens)
		{
			var stack = new Stack<int>();
			foreach (var token in tokens)
			{
				if (token.symbol >= '0' && token.symbol <= '9')
				{
					stack.Push(token.symbol - '0');
				}
				else
				{
					var a = stack.Pop();
					var b = stack.Pop();
					var c = 0;
					switch (token.symbol)
					{
						case (byte)'+':
							c = (b + a);
							break;
						case (byte)'-':
							c = (b - a);
							break;
						case (byte)'*':
							c = (b * a);
							break;
						case (byte)'/':
							c = (b / a);
							break;
						case (byte)'^':
							c = (int)Math.Pow(b, a);
							break;
						case (byte)'|':
							c = (b * 10);
							if (c < 0) c -= a; else c += a;
							break;
					}
					stack.Push(c);
				}
			}
			return stack.ToArray();
		}
	}
}
