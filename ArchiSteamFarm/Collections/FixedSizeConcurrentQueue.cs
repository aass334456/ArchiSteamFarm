//     _                _      _  ____   _                           _____
//    / \    _ __  ___ | |__  (_)/ ___| | |_  ___   __ _  _ __ ___  |  ___|__ _  _ __  _ __ ___
//   / _ \  | '__|/ __|| '_ \ | |\___ \ | __|/ _ \ / _` || '_ ` _ \ | |_  / _` || '__|| '_ ` _ \
//  / ___ \ | |  | (__ | | | || | ___) || |_|  __/| (_| || | | | | ||  _|| (_| || |   | | | | | |
// /_/   \_\|_|   \___||_| |_||_||____/  \__|\___| \__,_||_| |_| |_||_|   \__,_||_|   |_| |_| |_|
// |
// Copyright 2015-2019 Łukasz "JustArchi" Domeradzki
// Contact: JustArchi@JustArchi.net
// |
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// |
// http://www.apache.org/licenses/LICENSE-2.0
// |
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ArchiSteamFarm.Collections {
	internal sealed class FixedSizeConcurrentQueue<T> : IEnumerable<T> {
		private readonly ConcurrentQueue<T> Queue = new ConcurrentQueue<T>();

		internal byte MaxCount {
			get => BackingMaxCount;

			set {
				if (value == 0) {
					ASF.ArchiLogger.LogNullError(nameof(value));

					return;
				}

				BackingMaxCount = value;

				while ((Queue.Count > MaxCount) && Queue.TryDequeue(out _)) { }
			}
		}

		private byte BackingMaxCount;

		internal FixedSizeConcurrentQueue(byte maxCount) {
			if (maxCount == 0) {
				throw new ArgumentNullException(nameof(maxCount));
			}

			MaxCount = maxCount;
		}

		public IEnumerator<T> GetEnumerator() => Queue.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		internal void Enqueue(T obj) {
			Queue.Enqueue(obj);

			if (Queue.Count <= MaxCount) {
				return;
			}

			Queue.TryDequeue(out _);
		}
	}
}
