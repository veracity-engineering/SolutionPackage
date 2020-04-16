using System;
using System.Threading.Tasks;

namespace DNVGL.Common.Core.Patterns
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public interface IVisitor<in TElement>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        void Visit(TElement element);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        Task VisitAsync(TElement element);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IVisitor<in TElement, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        TResult Visit(TElement element);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        Task<TResult> VisitAsync(TElement element);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDrive"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    public abstract class VisitorBase<TDrive, TElement> : IVisitor<TElement>
        where TDrive : VisitorBase<TDrive, TElement>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void Visit(TElement element)
        {
            ((TDrive)this).DoVisit((dynamic)element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public async Task VisitAsync(TElement element)
        {
            await ((TDrive)this).DoVisitAsync((dynamic)element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public virtual void DoVisit(TElement element)
        {
            throw new NotImplementedException(
        $"Visitor (Visitor type: '{typeof(TDrive).FullName}') " +
                $"doesn't support Method (Method signature: 'void {nameof(DoVisit)}({typeof(TElement).Name} element)') " +
                $"for Element (Element type: '{typeof(TElement).FullName}')");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public virtual async Task DoVisitAsync(TElement element)
        {
            throw await Task.FromResult(
                new NotImplementedException(
            $"Visitor (Visitor type: '{typeof(TDrive).FullName}') " +
                    $"doesn't support Method (Method signature: 'Task {nameof(DoVisitAsync)}({typeof(TElement).Name} element)') " +
                    $"for Element (Element type: '{typeof(TElement).FullName}')"));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDrive"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class VisitorBase<TDrive, TElement, TResult> : IVisitor<TElement, TResult>
        where TDrive : VisitorBase<TDrive, TElement, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public TResult Visit(TElement element)
        {
            return ((TDrive)this).DoVisit((dynamic)element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public async Task<TResult> VisitAsync(TElement element)
        {
            return await ((TDrive)this).DoVisitAsync((dynamic)element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public virtual TResult DoVisit(TElement element)
        {
            throw new NotImplementedException(
        $"Visitor (Visitor type: '{typeof(TDrive).FullName}') " +
                $"doesn't support Method (Method signature: '{typeof(TResult).Name} {nameof(DoVisit)}({typeof(TElement).Name} element)') " +
                $"for Element (Element type: '{typeof(TElement).FullName}')");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public virtual async Task<TResult> DoVisitAsync(TElement element)
        {
            throw await Task.FromResult(
                new NotImplementedException(
            $"Visitor (Visitor type: '{typeof(TDrive).FullName}') " +
                    $"doesn't support Method (Method signature: 'Task<{typeof(TResult).Name}> {nameof(DoVisitAsync)}({typeof(TElement).Name} element)') " +
                    $"for Element (Element type: '{typeof(TElement).FullName}')"));
        }
    }
}
