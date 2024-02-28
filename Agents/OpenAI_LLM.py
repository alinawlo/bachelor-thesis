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
Using Unity, please provide a step-by-step guide on creating a scene of a {scene_description} using the
prefabs listed in the {assets_list}. Please suggest the overall size, dimensions of the scene's elements, and the arrangement of the elements.
The guide should also be as technical and detailed as possible, without the need for Unity Editor Scripts.  


In your guide, please ensure that all steps are formulated as follows:
1. Clearly state the action to be performed (e.g., "Create a tree using Editor/Tree_01.prefab").
2. Provide specific instructions on where to click and what to select.
3. Specify the values for the element's scale (e.g., "Set the and Scale to (1, 0.1, 1) for the first pathway segment").
4. Specify the values for the element's  transform position (e.g., "Set the Transform Position to (0, 1, 0)").
5. If element should be on the ground in a realistic situation set its y-Axis position value to 0.89.
6. If is supposed to be the ground, set its y-Axis position value to 0.
7. Provide a substep for elements written in plural that need to be duplicated (e.g., "Duplicate 'Pathway1' in the Hierarchy to create more pathway segments, adjusting their Transform Positions to create a continuous pathway through the garden").
8. Make sure to use the same asset for duplicate elements.
9. Avoid overlapping of different elements.
10. Do not include subtitles within each step.
11. Make sure that all elements are attached to the ground and are not flying around.
12. The response should not require the need for Unity Editor Scripts or any specific functionality, interactivity, or navigation options.
13. Always opt to use prefabs rather than creating the objects from scratch.
14. Do not duplicate the element if the prompt expliticitly says to create a single instance of the element.
16. Adjust the lighing and the camera position of the scene.
15. Do not provide an introduction to the results.


Please follow this format for each step in your guide to ensure clarity and consistency.

after generating the steps, format each step like this code: 
/*step*/
@"Create the Ball:
a. Right-click in the Hierarchy.
b. Select 3D Object > Sphere. This will create a spherical object in your scene.
d. Set the Transform Position to (0, 1, 0) to place the ball at the center of the scene.
e. Set the Scale to (3, 3, 3) to make it a big ball.
f. In the Inspector window, click on the Sphere's Mesh Renderer component.
g. Click on the color box next to the Albedo in the material settings.
h. Set the color to a blue shade (e.g., RGB: 0, 0, 255) to make it appear as a big blue ball."

so before each @ /*step*/ should be typed, except for the first step. It should start with the @.
"""

# Create a prompt template
prompt_temp = PromptTemplate(input_variables=["scene_description", "assets_list"], template=scene_template)

# Define the scene description
#description = "a simple scene of a garden using cubes and spheres as 3D objects, with no specific color scheme. The garden should include grass, simple plants, simple trees, a pathway, and lanterns next to the pathway. The scene should be set during daytime."
if len(sys.argv) > 1:
    description = sys.argv[1]
else:
    description = "a simple scene of a garden and a blue ball"

assets_list_string = sys.argv[2] if len(sys.argv) > 2 else "cubes,spheres"

# Split the assets list string into an array
assets_list = assets_list_string.split(',')

# Format the assets list for the prompt
formatted_assets_list = ', '.join([f'"{asset}"' for asset in assets_list])

# Format the prompt with the scene description and assets list
formatted_prompt = prompt_temp.format(scene_description=description, assets_list=formatted_assets_list)

# Initialize the language model chain
chain = LLMChain(llm=llm, prompt=prompt_temp)

# Generate the guide by passing a dictionary
input_dict = {"scene_description": description, "assets_list": formatted_assets_list}
generated_guide = chain.run(input_dict)

# Print each step
print(generated_guide)