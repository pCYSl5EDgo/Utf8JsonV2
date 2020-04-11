// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json
{
    public enum CollectionDeserializeToBehaviour
    {
        Add, // default is add(protobuf-merge, json.net-populateobject
        OverwriteReplace
    }
}