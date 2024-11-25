namespace Xsolla.Core.Editor
{
	public interface IFindCriteria<in T>
	{
		bool MatchesCriteria(T obj);
	}
}