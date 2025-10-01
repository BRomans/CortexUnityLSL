"""
LSL Simulator for CortexInference output
Sends randomized state vectors every second
"""
import time
import random
from pylsl import StreamInfo, StreamOutlet, cf_float32
import argparse


def create_lsl_outlet():
    """
    Create an LSL outlet for CortexInference data.
    """
    # Define the stream
    info = StreamInfo(
        name='CortexInference',
        type='EEG',  # or 'EEG', 'Cognitive', depending on your use case
        channel_count=4,  # arousal, valence, focus, calm
        nominal_srate=1,  # 1 Hz (once per second)
        channel_format=cf_float32,
        source_id='cortex_simulator_001'
    )

    # Add channel metadata
    channels = info.desc().append_child("channels")
    for label in ["arousal", "valence", "focus", "calm"]:
        ch = channels.append_child("channel")
        ch.append_child_value("label", label)
        ch.append_child_value("unit", "normalized")
        ch.append_child_value("type", "cognitive_state")

    # Create the outlet
    outlet = StreamOutlet(info)
    print("LSL Outlet 'CortexInference' created successfully!")
    print("Stream info:")
    print(f"  Name: {info.name()}")
    print(f"  Type: {info.type()}")
    print(f"  Channels: {info.channel_count()}")
    print(f"  Sample Rate: {info.nominal_srate()} Hz")
    print("\nStreaming data...\n")

    return outlet


def generate_random_state():
    """
    Generate a random cognitive state vector.
    Values are between 0 and 1 (normalized).
    """
    return {
        "arousal": random.uniform(0.0, 1.0),
        "valence": random.uniform(0.0, 1.0),
        "focus": random.uniform(0.0, 1.0),
        "calm": random.uniform(0.0, 1.0)
    }


def generate_realistic_state(previous_state=None, smoothing=0.7):
    """
    Generate a more realistic cognitive state with smooth transitions.

    Args:
        previous_state: Previous state dict for smooth interpolation
        smoothing: How much to retain from previous state (0-1)
    """
    new_state = generate_random_state()

    if previous_state is not None:
        # Smooth transition from previous state
        for key in new_state:
            new_state[key] = (
                    smoothing * previous_state[key] +
                    (1 - smoothing) * new_state[key]
            )

    return new_state


def main(wait_time=1.0):
    """
    Main loop: create outlet and stream data every second.
    """
    # Create LSL outlet
    outlet = create_lsl_outlet()

    # Initial state
    current_state = generate_random_state()

    try:
        sample_count = 0
        while True:
            # Generate new state (with smooth transitions)
            current_state = generate_realistic_state(current_state, smoothing=0.7)

            # Prepare data as list (in order: arousal, valence, focus, calm)
            sample = [
                current_state["arousal"],
                current_state["valence"],
                current_state["focus"],
                current_state["calm"]
            ]

            # Send sample
            outlet.push_sample(sample)

            # Print current state
            sample_count += 1
            print(f"Sample #{sample_count}: {current_state}")

            # Wait 1 second
            time.sleep(wait_time)

    except KeyboardInterrupt:
        print("\n\nStream stopped by user.")
        print(f"Total samples sent: {sample_count}")


if __name__ == "__main__":
    print("=" * 60)
    print("CortexInference LSL Simulator")
    print("=" * 60)
    print("\nThis script simulates cognitive state output and streams it via LSL.")
    print("Press Ctrl+C to stop.\n")

    # get argument from command line for wait time
    parser = argparse.ArgumentParser(description="CortexInference LSL Simulator")
    parser.add_argument(
        "--wait",
        type=float,
        default=1.0,
        help="Time to wait between samples (in seconds). Default is 1.0"
    )
    args = parser.parse_args()
    wait_time = args.wait

    # Check if pylsl is installed
    try:
        import pylsl

        print(f"pylsl version: {pylsl.__version__}\n")
    except ImportError:
        print("ERROR: pylsl not installed!")
        print("Install it with: pip install pylsl\n")
        exit(1)

    main(wait_time=wait_time)