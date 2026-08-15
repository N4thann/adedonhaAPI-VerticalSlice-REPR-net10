using AdedonhaAPI.Application.Common.Context;
using Shouldly;

namespace AdedonhaAPI.tests.Common.Context
{
    public class OperationContextTests
    {
        [Fact(DisplayName = "SUCESSO - Deve retornar o valor definido por Set enquanto nao for limpo")]
        public void Set_WhenCalled_ShouldMakeValueAvailableViaCurrent()
        {
            // Arrange
            var operationId = Guid.NewGuid().ToString();

            // Act
            OperationContext.Set(operationId);

            // Assert
            OperationContext.Current.ShouldBe(operationId);

            OperationContext.Clear();
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar null apos Clear")]
        public void Clear_WhenCalledAfterSet_ShouldResetCurrentToNull()
        {
            // Arrange
            OperationContext.Set(Guid.NewGuid().ToString());

            // Act
            OperationContext.Clear();

            // Assert
            OperationContext.Current.ShouldBeNull();
        }

        [Fact(DisplayName = "SUCESSO - Deve isolar o valor entre fluxos assincronos distintos")]
        public async Task Set_WhenCalledInDifferentAsyncFlows_ShouldNotLeakBetweenThem()
        {
            // Arrange
            string? capturedInSecondFlow = null;

            // Act
            var firstFlow = Task.Run(async () =>
            {
                OperationContext.Set("flow-1");
                await Task.Delay(50);
                OperationContext.Current.ShouldBe("flow-1");
                OperationContext.Clear();
            });

            var secondFlow = Task.Run(async () =>
            {
                await Task.Delay(10);
                OperationContext.Set("flow-2");
                await Task.Delay(50);
                capturedInSecondFlow = OperationContext.Current;
                OperationContext.Clear();
            });

            await Task.WhenAll(firstFlow, secondFlow);

            // Assert
            capturedInSecondFlow.ShouldBe("flow-2");
        }
    }
}
