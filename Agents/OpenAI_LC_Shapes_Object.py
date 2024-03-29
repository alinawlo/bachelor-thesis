import sys
import os
from langchain.llms import OpenAI
from langchain.prompts import PromptTemplate
from langchain.chains import LLMChain
from langchain.chat_models import ChatOpenAI


# Set the OpenAI API key
os.environ['OPENAI_API_KEY'] = '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'

# Initialize the OpenAI model
#llm = OpenAI(model_name='text-davinci-003', temperature=0.9, max_tokens=1000)
llm= ChatOpenAI(model_name='gpt-4', temperature=0.3, max_tokens=3000)


# Define the scene template
scene_template = """
Using Unity, please provide a step-by-step guide on creating only the {object_description} using a using cubes and spheres as 3D objects. Please suggest the overall  size, dimensions of the scene's elements, and the arrangement of the elements. The guide should also be as technical and detailed as possible, without the need for Unity Editor Scripts. 

In your guide, please ensure that all steps are formulated as follows: 

1. Clearly state the action to be performed (e.g., "Create a pathway using cubes"). 
2. Provide specific instructions on where to click and what to select (e.g., "Right-click in the Hierarchy, select 3D Object > Cube").
3. Include steps for renaming and tagging the element (e.g., "Rename and tag the Cube to 'Pathway1'"). 
4. Specify the values for the element's scale, and color (e.g., "Set the and Scale to (1, 0.1, 1) for the first pathway segment"). 
5. Specify the values for the element's transform position, please keep in mind abiding the laws of gravity (e.g., "Set the Transform Position to (0, 0.1, 0)") 
6. Provide a substep for elements written in plural that need to be duplicated (e.g., "Duplicate 'Pathway1' in the Hierarchy to create more pathway segments, adjusting their Transform Positions to create a continuous pathway through the garden"). 
7. Ensure that all shapes to create a certain element are attached to each other. 
8. Avoid overlapping of different elements. 
9. Do not include subtitles within each step. 
10. Make sure that all elements are on top of the ground. 
11. The response should not require the need for Unity Editor Scripts or any specific functionality, interactivity, or navigation options. 

Please follow this format for each step in your guide to ensure clarity and consistency.

after generating the steps, format each step like this code: 
@"Create the ground (grass):
a. Right-click in the Hierarchy, select 3D Object > Cube.
b. Rename and tag the Cube to ""Ground"".
c. Set the Transform Position to (0, 0, 0) and Scale to (10, 0.1, 10).
d. In the Inspector window, click on the Cube's Mesh Renderer component, then click on the color box next to the Albedo in the material settings. Set the color to a suitable grass color (e.g., RGB: 0, 200, 0)."

"""


# Create a prompt template
prompt_temp = PromptTemplate(input_variables=["scene_description"], template=scene_template)

# Define the scene description
if len(sys.argv) > 1:
    description = sys.argv[1]
else:
    description = "a simple scene of a garden and a blue ball"

# Format the prompt with the scene description
formatted_prompt = prompt_temp.format(scene_description=description)

# Initialize the language model chain
chain = LLMChain(llm=llm, prompt=prompt_temp)

#  Generate the guide
generated_guide = chain.run(description)

# Print each step
print(generated_guide)