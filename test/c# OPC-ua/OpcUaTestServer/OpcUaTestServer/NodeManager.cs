﻿using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaTestServer
{
    internal class NodeManager : INodeManager
    {
        public IEnumerable<string> NamespaceUris => throw new NotImplementedException();

        public void AddReferences(IDictionary<NodeId, IList<IReference>> references)
        {
            throw new NotImplementedException();
        }

        public void Browse(OperationContext context, ref ContinuationPoint continuationPoint, IList<ReferenceDescription> references)
        {
            throw new NotImplementedException();
        }

        public void Call(OperationContext context, IList<CallMethodRequest> methodsToCall, IList<CallMethodResult> results, IList<ServiceResult> errors)
        {
            throw new NotImplementedException();
        }

        public ServiceResult ConditionRefresh(OperationContext context, IList<IEventMonitoredItem> monitoredItems)
        {
            throw new NotImplementedException();
        }

        public void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            throw new NotImplementedException();
        }

        public void CreateMonitoredItems(OperationContext context, uint subscriptionId, double publishingInterval, TimestampsToReturn timestampsToReturn, IList<MonitoredItemCreateRequest> itemsToCreate, IList<ServiceResult> errors, IList<MonitoringFilterResult> filterErrors, IList<IMonitoredItem> monitoredItems, ref long globalIdCounter)
        {
            throw new NotImplementedException();
        }

        public void DeleteAddressSpace()
        {
            throw new NotImplementedException();
        }

        public void DeleteMonitoredItems(OperationContext context, IList<IMonitoredItem> monitoredItems, IList<bool> processedItems, IList<ServiceResult> errors)
        {
            throw new NotImplementedException();
        }

        public ServiceResult DeleteReference(object sourceHandle, NodeId referenceTypeId, bool isInverse, ExpandedNodeId targetId, bool deleteBidirectional)
        {
            throw new NotImplementedException();
        }

        public object GetManagerHandle(NodeId nodeId)
        {
            throw new NotImplementedException();
        }

        public NodeMetadata GetNodeMetadata(OperationContext context, object targetHandle, BrowseResultMask resultMask)
        {
            throw new NotImplementedException();
        }

        public void HistoryRead(OperationContext context, HistoryReadDetails details, TimestampsToReturn timestampsToReturn, bool releaseContinuationPoints, IList<HistoryReadValueId> nodesToRead, IList<HistoryReadResult> results, IList<ServiceResult> errors)
        {
            throw new NotImplementedException();
        }

        public void HistoryUpdate(OperationContext context, Type detailsType, IList<HistoryUpdateDetails> nodesToUpdate, IList<HistoryUpdateResult> results, IList<ServiceResult> errors)
        {
            throw new NotImplementedException();
        }

        public void ModifyMonitoredItems(OperationContext context, TimestampsToReturn timestampsToReturn, IList<IMonitoredItem> monitoredItems, IList<MonitoredItemModifyRequest> itemsToModify, IList<ServiceResult> errors, IList<MonitoringFilterResult> filterErrors)
        {
            throw new NotImplementedException();
        }

        public void Read(OperationContext context, double maxAge, IList<ReadValueId> nodesToRead, IList<DataValue> values, IList<ServiceResult> errors)
        {
            throw new NotImplementedException();
        }

        public void SetMonitoringMode(OperationContext context, MonitoringMode monitoringMode, IList<IMonitoredItem> monitoredItems, IList<bool> processedItems, IList<ServiceResult> errors)
        {
            throw new NotImplementedException();
        }

        public ServiceResult SubscribeToAllEvents(OperationContext context, uint subscriptionId, IEventMonitoredItem monitoredItem, bool unsubscribe)
        {
            throw new NotImplementedException();
        }

        public ServiceResult SubscribeToEvents(OperationContext context, object sourceId, uint subscriptionId, IEventMonitoredItem monitoredItem, bool unsubscribe)
        {
            throw new NotImplementedException();
        }

        public void TransferMonitoredItems(OperationContext context, bool sendInitialValues, IList<IMonitoredItem> monitoredItems, IList<bool> processedItems, IList<ServiceResult> errors)
        {
            throw new NotImplementedException();
        }

        public void TranslateBrowsePath(OperationContext context, object sourceHandle, RelativePathElement relativePath, IList<ExpandedNodeId> targetIds, IList<NodeId> unresolvedTargetIds)
        {
            throw new NotImplementedException();
        }

        public void Write(OperationContext context, IList<WriteValue> nodesToWrite, IList<ServiceResult> errors)
        {
            throw new NotImplementedException();
        }
    }
}
