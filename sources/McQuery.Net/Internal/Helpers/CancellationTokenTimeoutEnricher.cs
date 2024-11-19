using System.Diagnostics.CodeAnalysis;

namespace McQuery.Net.Internal.Helpers;

public static class CancellationTokenTimeoutEnrichHelper
{
    public static CancellationTokenSourceWithTimeout ToSourceWithTimeout(this CancellationToken token, TimeSpan timeout)
        => CancellationTokenSourceWithTimeout.Create(token, timeout);

    public record struct CancellationTokenSourceWithTimeout : IDisposable
    {
        private readonly CancellationTokenSource _originSource;
        private readonly CancellationTokenSource _linkedSource;

        public CancellationToken Token => _linkedSource.Token;

        private CancellationTokenSourceWithTimeout(CancellationTokenSource originSource, CancellationTokenSource linkedSource)
        {
            _originSource = originSource;
            _linkedSource = linkedSource;
        }

        [SuppressMessage("Design", "CA1068:CancellationToken parameters must come last")]
        public static CancellationTokenSourceWithTimeout Create(CancellationToken cancellationToken, TimeSpan timeout)
        {
            CancellationTokenSource timeoutSource = new(timeout);
            return new CancellationTokenSourceWithTimeout(
                timeoutSource,
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    timeoutSource.Token));
        }

        private bool _isDisposed = false;

        public void Dispose()
        {
            if (_isDisposed) return;
            _originSource.Dispose();
            _linkedSource.Dispose();
            _isDisposed = true;
        }
    }
}
